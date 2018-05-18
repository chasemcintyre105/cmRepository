using System;
using RuleEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RuleEngineAddons.TurnBased;

namespace RuleEngineAddons.RulePanel {

    public class RulePanelManager : IManager {

        public TurnManager TurnManager;
        public PanelManager PanelManager;
        public ActionStackManager ActionStackManager;
        public RulePanelController RulePanelController;
        public NotificationManager NotificationManager;

        public Dictionary<string, RulePanelRenderer> renderersByName { get; protected set; }
        public bool firstOpeningOfRulePanel;

        public RulePanelRenderer MainRulePanel;
        public GameObject RulePanelMovable;

        // Type function lists
        public Dictionary<Type, List<RuleComponent>> ArgumentTypeToReturnType { get; protected set; }
        protected List<RuleComponent> SelectableObjects;
        protected Dictionary<RuleType, List<Type>> SelectionOptionExclusions;
        protected Dictionary<RuleType, List<Variable>> SelectableVariables;

        protected RulePanelRenderer[] allPanels;

        public override void Preinit() {
            renderersByName = new Dictionary<string, RulePanelRenderer>();
            firstOpeningOfRulePanel = true;

            ArgumentTypeToReturnType = new Dictionary<Type, List<RuleComponent>>();
            SelectionOptionExclusions = new Dictionary<RuleType, List<Type>>();
            SelectableVariables = new Dictionary<RuleType, List<Variable>>();
            SelectableObjects = new List<RuleComponent>();

            ArgumentTypeToReturnType.Add(typeof(VoidStatement), new List<RuleComponent>()); // For statements

        }

        public override void Init() {

            TurnManager = E.GetManager<TurnManager>();
            PanelManager = E.GetManager<PanelManager>();
            ActionStackManager = E.GetManager<ActionStackManager>();
            NotificationManager = E.GetManager<NotificationManager>();
            RulePanelController = E.GetController<RulePanelController>();

            PanelManager.GetRectTransform(RulePanelController.OrganisationalObjects.PanelsContainer).pivot = new Vector2(0.5f, 0.5f);

            allPanels = GameObject.FindObjectsOfType<RulePanelRenderer>();

            foreach (RulePanelRenderer panel in allPanels) {
                Canvas[] canvasses = panel.gameObject.GetComponentsInParent<Canvas>();
                Canvas canvas = canvasses[canvasses.Length - 1]; // Get top most canvas
                GameObject RulePanelGO = panel.gameObject;
                GameObject PanelContainer = panel.transform.parent.gameObject;

                Transform PanelParent = PanelContainer.transform;

                renderersByName.Add(panel.PanelName, panel);
                panel.SetCanvas(canvas);
                panel.SetRulePanelManager(this);

                PanelDrawingAttachment drawingAttachment;
                RectTransform RulePanelRect;
                PanelCreator RulePanelConfigurer = new PanelCreator(PanelManager, RulePanelGO)
                    .SetName("Rule panel: " + panel.PanelName)
                    .ConfigureWithDrawingAttachment(out drawingAttachment);

                if (panel.Behaviour.Scrollable) {
                    GameObject ScrollContainer;
                    RectTransform ScrollContainerRect;
                    ScrollRect scrollRect;
                    PanelCreator ScrollContainerCreator = new PanelCreator(PanelManager)
                        .SetName("Scroll container")
                        .SetParent(PanelParent)
                        .SetChild(panel.gameObject)
                        .GetRectTransform(out ScrollContainerRect)
                        .SetPivot(0.5f, 1f)
                        .ConfigureWithDrawingAttachment()

                        // Make the scroll container transparent
                        .SafelyGetComponent(delegate (Image i) {
                            i.color = Color.clear;
                        })

                        // Add a scroll rect to allow the default Unity scrolling
                        .SafelyGetComponent(out scrollRect, delegate (ScrollRect sr) {
                            sr.content = ((RectTransform) RulePanelGO.transform);
                            sr.horizontal = false;
                            sr.scrollSensitivity = 10;
                        });

                    if (panel.Appearance.FullScreen)
                        ScrollContainerCreator.SetAsFullScreen(canvas);

                    ScrollContainerCreator
                        .SetVisibility(true)
                        .Finalise(out ScrollContainer);

                    // Set the new panel parent since the scroll container is being inserted inbetween
                    PanelParent = ScrollContainerRect;

                    // Make scroll bar if template is set
                    if (panel.Appearance.ScrollBarTemplate != null) {
                        SetupScrollbar(panel, PanelParent, scrollRect);
                    }

                }

                RulePanelConfigurer

                    // Add a grid layout component to the rule panel to organise the rules and headings
                    .SafelyGetComponent(delegate (VerticalLayoutGroup RuleLayout) {
                        RuleLayout.childForceExpandHeight = false;
                        RuleLayout.padding = new RectOffset(15, 15, 15, 15);
                        RuleLayout.spacing = 10;
                    })

                    // Add two size fitters and a layout element so that the rule panel contains the rules and has a minimum height of the screen
                    .SafelyGetComponent(delegate (ContentSizeFitter RulePanelFitter) {
                        RulePanelFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
                        RulePanelFitter = RulePanelGO.AddComponent<ContentSizeFitter>();
                        RulePanelFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    })

                    // Set the minimum height of each element in the vertical layout group
                    .SafelyGetComponent(delegate (LayoutElement ruleLayoutElement) {
                        ruleLayoutElement.minHeight = canvas.pixelRect.height;
                    })

                    // Set appearance
                    .SafelyGetComponent(delegate (Image RulePanelImage) {
                        if (panel.Appearance.Background != null)
                            RulePanelImage.sprite = panel.Appearance.Background;
                        RulePanelImage.type = Image.Type.Tiled;
                        RulePanelImage.color = panel.Appearance.Color;
                    })

                    // Configure position parameters
                    .GetRectTransform(out RulePanelRect, delegate (RectTransform transform) {
                        transform.anchorMin = new Vector2(0, 1);
                        transform.anchorMax = new Vector2(0, 1);
                        transform.sizeDelta = new Vector2(canvas.pixelRect.width, canvas.pixelRect.height);
                    })

                    .Finalise();

                // Update the width of the rule panel
                drawingAttachment.PanelWidth = canvas.pixelRect.width;

                RulePanelRect.pivot = new Vector2(0.5f, 1);
                RulePanelRect.localPosition = Vector2.zero;

                SetupBackdrop(panel, PanelParent.gameObject);

            }

            // Set the main panel if it exists
            renderersByName.TryGetValue("Main", out MainRulePanel);

            RulePanelMovable = MainRulePanel.transform.parent.gameObject;

            // Close the panel
            RulePanelMovable.transform.localPosition = new Vector3(0, RulePanelMovable.transform.localPosition.y);

            // Configure the default rule panel open and close functions
            if (TurnManager == null) {
                MainRulePanel.DecideToOpen = delegate {
                    
                    // View the rule panel
                    PanelManager.GUIStateMachine.IssueCommand(GUIEvent.View_Rules);

                };
            } else {
                MainRulePanel.DecideToOpen = delegate {
                    // View the rule panel, but only if the current game state is waiting
                    if (TurnManager.TurnStateMachine.CurrentState == TurnState.Waiting)
                        PanelManager.GUIStateMachine.IssueCommand(GUIEvent.View_Rules);

                    // Otherwise leave it close
                };
            }
            
            MainRulePanel.DecideToClose = delegate {

                // Begin the process of checking the state of the rules to eventually close the panel
                PanelManager.GUIStateMachine.IssueCommand(GUIEvent.Check_Rules);
                
            };

            // Set click function for the rule panel selection backdrop to close the selection panel
            PanelManager.MakeGameObjectClickable(MainRulePanel.SelectionBackdrop, GUIEvent.Cancel_Selection, false, null, null, null);

        }

        public virtual void RedrawAllPanels() {
            foreach (RulePanelRenderer panel in allPanels) {
                RefreshRulePanel(panel);
            }
        }

        public virtual void SetAllPanelsToDefaultPosition() {
            foreach (RulePanelRenderer panel in allPanels) {
                panel.Init();
                if (panel.Behaviour.IsOpenByDefault) {
                    panel.Open();
                } else {
                    panel.Close();
                }
            }
        }

        public virtual void SetupBackdrop(RulePanelRenderer panel, GameObject parent) {

            GameObject backdrop = null;

            if (panel.Appearance.SelectionBackdropTemplate != null) {
                backdrop = PanelManager.NewInstance(panel.Appearance.SelectionBackdropTemplate, parent);
            } else {
                backdrop = new GameObject();
            }

            new PanelCreator(PanelManager, backdrop)
                .SetName("Selection Backdrop")
                .SetParent(parent)
                .SetToFillParent()
                .SafelyGetComponent(delegate (Image Image) {
                    Image.color = new Color(0.33f, 0.33f, 0.33f, 0.66f);
                })
                .GetRectTransform(delegate (RectTransform transform) {
                    transform.localPosition = Vector3.zero;
                })
                .SetVisibility(false)
                .Finalise();

            panel.SetSelectionBackdrop(backdrop);

        }

        public virtual void SetupScrollbar(RulePanelRenderer panel, Transform parent, ScrollRect scrollRect) {

            RectTransform ScrollbarRect;
            new PanelCreator(PanelManager, GameObject.Instantiate(panel.Appearance.ScrollBarTemplate.gameObject))
                .SetParent(parent)
                .SafelyGetComponent(delegate (Scrollbar scrollbar) {
                    panel.SetScrollbar(scrollbar);
                    scrollRect.verticalScrollbar = scrollbar;
                })
                .GetRectTransform(out ScrollbarRect, delegate (RectTransform transform) {
                    transform.pivot = new Vector2(1, 0.5f);
                    transform.anchorMin = new Vector2(1, 0);
                    transform.anchorMax = new Vector2(1, 1);
                    transform.sizeDelta = new Vector3(transform.rect.width, 0);
                })
                .SetVisibility(true)
                .Finalise();

            RectTransform parentTransform = parent.GetComponent<RectTransform>();
            if (parentTransform != null)
                ScrollbarRect.localPosition = new Vector3(parentTransform.rect.width / 2, -parentTransform.rect.height / 2, 0);

        }

        public virtual void RefreshMainRulePanel() {
            MainRulePanel.Redraw();
        }

        public virtual void ToggleMainRulePanel() {
            MainRulePanel.ToggleVisibility();
        }

        public virtual void RefreshRulePanel(RulePanelRenderer panel, Rule toBeLeftExpanded = null) {

            // Clear the previous clickable GUI elements with associated engine object 
            PanelManager.Buttons.Clear();

            // Get the panel to render the rules to
            GameObject panelObj = panel.gameObject;
            PanelDrawingAttachment panelAttachment = panel.GetComponent<PanelDrawingAttachment>();
            Assert.NotNull("panelAttachment", panelAttachment);

            // Reset the panel's drawing properties
            PanelManager.ResetPanel(panelObj);

            // Rerender the rules
            IEnumerable<RuleType> RuleTypes = E.RuleManager.GetAllRuleTypes();
            int count = 0;
            foreach (RuleType RuleType in RuleTypes) {
                count++;
                bool isLast = E.RuleManager.RuleTypeCount() == count;

                DrawInContainer(delegate (PanelDrawingAttachment container) {

                    // Write the heading
                    PanelManager.WriteTextToPanel(container, panelAttachment.TitleTextTemplate, RuleType.ID + " rules", container.gameObject);
                    PanelManager.WriteNewLineToPanel(container, false, -20);

                }, panelObj.transform, panelAttachment.PanelWidth, Color.clear);

                Rule CurrentRule = null;
                bool NotAccessingCurrentRule = false;

                for (int RuleIndex = 0; RuleIndex < E.RuleManager.RuleTypeToList[RuleType].Count; RuleIndex++) {
                    CurrentRule = E.RuleManager.RuleTypeToList[RuleType][RuleIndex];

                    Dictionary<string, object> RuleToggleProperties = null;

                    // Decide whether the current rule is modifiable or not
                    if (E.RuleManager.RuleJudge.IsAccessingARule())
                        NotAccessingCurrentRule = CurrentRule != E.RuleManager.RuleJudge.GetAccessedRule();
                    else
                        NotAccessingCurrentRule = false;

                    DrawInContainer(delegate (PanelDrawingAttachment container) {

                        // Remove rule button
                        if (!NotAccessingCurrentRule &&
                            E.RuleManager.RuleJudge.IsRemovingARuleAllowed() &&
                            !CurrentRule.Irremovalable) {

                            Dictionary<string, object> properties = PanelManager.WriteButtonToPanel(container, panelAttachment.RemoveButtonTemplate, GUIEvent.Remove_Rule, CurrentRule, null, null);
                            properties.Add("Rule", CurrentRule);
                            properties.Add("RuleIndex", RuleIndex);
                            properties.Add("RuleType", RuleType);

                        }

                        // Rule name
                        if (CurrentRule.Name != null)
                            RuleToggleProperties = PanelManager.WriteTextToPanel(container, panelAttachment.TitleTextTemplate, E.RuleManager.RuleTypeToList[RuleType][RuleIndex].Name, container.gameObject, GUIEvent.Toggle_Rule_Visible, true);
                        else
                            RuleToggleProperties = PanelManager.WriteTextToPanel(container, panelAttachment.TitleTextTemplate, RuleType.ToString() + " rule " + RuleIndex, container.gameObject, GUIEvent.Toggle_Rule_Visible, true);

                        PanelManager.WriteNewLineToPanel(container, false, -10);

                    }, panelObj.transform, panelAttachment.PanelWidth, Color.clear);


                    DrawInContainer(delegate (PanelDrawingAttachment container) {

                        // Render the rule to the panel
                        RuleRenderingExecutor renderer = new RuleRenderingExecutor(E, container.gameObject);
                        PanelManager.RenderSingleRule(renderer, RuleType, RuleIndex, container.gameObject, RuleIndex == E.RuleManager.RuleTypeToList[RuleType].Count - 1, NotAccessingCurrentRule);

                        PanelManager.WriteNewLineToPanel(container, false, 0);

                        // Register this container with the toggle properties, so that when the rule name is clicked, it knows what to toggle
                        RuleToggleProperties.Add("Container", container.gameObject);

                        // Determine if this rule must be minimised by default
                        if (!CurrentRule.SomeElementsEditable &&
                            (NotAccessingCurrentRule ||
                            !E.RuleManager.RuleJudge.IsRemovingARuleAllowed() ||
                            CurrentRule.Locked)) {

                            container.gameObject.SetActive(false);

                        }

                    }, panelObj.transform, panelAttachment.PanelWidth);

                }

                // Write new rule button
                if (!NotAccessingCurrentRule &&
                    !panelAttachment.AddRuleRestricted &&
                    !E.RuleManager.NoAdditionalRules[RuleType] &&
                    E.RuleManager.RuleJudge.IsNewRuleAllowed() &&
                    isLast) {

                    DrawInContainer(delegate (PanelDrawingAttachment container) {

                        Dictionary<string, object> properties = PanelManager.WriteTextToPanel(container, panelAttachment.TextTemplate, "New " + RuleType.ID + " rule", container.gameObject, GUIEvent.Add_Rule);
                        properties.Add("RuleType", RuleType);
                        
                        PanelManager.WriteHorizontalSpaceToPanel(container, 5f);

                        properties = PanelManager.WriteButtonToPanel(container, panelAttachment.AddButtonTemplate, GUIEvent.Add_Rule, CurrentRule, null, null);
                        properties.Add("RuleType", RuleType);
                        
                        PanelManager.WriteNewLineToPanel(container, false, -10);

                    }, panelObj.transform, panelAttachment.PanelWidth, Color.clear);

                }

                // Update the height of the rule panel to take account of the new content
                PanelManager.UpdatePanelHeight(panelAttachment, 0);

            }

        }

        public virtual void DrawInContainer(Action<PanelDrawingAttachment> DrawingCallback, Transform parent, float width, Color? containerBackground = null) {

            PanelDrawingAttachment container = GameObject.Instantiate(RulePanelController.GUITemplates.ContainerTemplate).AddComponent<PanelDrawingAttachment>();
            PanelManager.ConfigurePanelDrawingAttachment(container);
            container.Reset();
            container.transform.SetParent(parent);
            container.PanelWidth = width;
            LayoutElement layoutElement = container.gameObject.AddComponent<LayoutElement>();

            // Set background colour
            if (containerBackground.HasValue)
                container.GetComponent<Image>().color = containerBackground.Value;

            DrawingCallback.Invoke(container);

            layoutElement.preferredHeight = container.VerticalOffset + container.PanelPadding;

        }

        public virtual void RegisterSelectableObject(RuleComponent obj) {

            SelectableObjects.Add(obj);

            // Add new argument type keys to ArgumentTypeToReturnType dictionary
            IterateThroughAcceptableArgumentTypes(obj, delegate (Type argumentType) {
                if (!ArgumentTypeToReturnType.ContainsKey(argumentType) && argumentType != typeof(Block)) {
                    ArgumentTypeToReturnType.Add(argumentType, new List<RuleComponent>());
                }
            });

            // Register this with ArgumentTypeToReturnType
            Type objReturnType = obj.GetReturnType();
            foreach (Type argumentType in ArgumentTypeToReturnType.Keys) {
                if (TypeFunctions.IsSameOrSubclassOf(objReturnType, argumentType) && argumentType != typeof(Block))
                    ArgumentTypeToReturnType[argumentType].Add(obj);
            }

        }

        public virtual void RegisterSelectionOptionExclusion(RuleType et, Type engineObjType) {
            Assert.True("Type is a type of RuleComponent", engineObjType.IsSubclassOf(typeof(RuleComponent)));

            List<Type> typeList = null;
            if (!SelectionOptionExclusions.TryGetValue(et, out typeList)) {
                typeList = new List<Type>();
                SelectionOptionExclusions.Add(et, typeList);
            }

            Assert.True("RuleComponent type is not yet registered", !typeList.Contains(engineObjType));
            typeList.Add(engineObjType);

        }

        public virtual void DeregisterSelectionOptionExclusion(RuleType et, Type engineObjType) {
            List<Type> typeList = null;
            if (!SelectionOptionExclusions.TryGetValue(et, out typeList)) {
                typeList = new List<Type>();
                SelectionOptionExclusions.Add(et, typeList);
            }

            Assert.True("RuleComponent type is already registered", typeList.Contains(engineObjType));
            typeList.Remove(engineObjType);

        }

        public virtual void RegisterSelectableVariable(RuleType et, Variable variable) {
            List<Variable> variables = null;
            if (!SelectableVariables.TryGetValue(et, out variables)) {
                variables = new List<Variable>();
                SelectableVariables.Add(et, variables);
            }

            Assert.True("Variable is not yet registered", !variables.Contains(variable));
            variables.Add(variable);

        }

        public virtual void DeregisterSelectableVariable(RuleType et, Variable variable) {
            List<Variable> variables = null;
            if (!SelectableVariables.TryGetValue(et, out variables)) {
                variables = new List<Variable>();
                SelectableVariables.Add(et, variables);
            }

            Assert.True("Variable is already registered", variables.Contains(variable));
            variables.Remove(variable);

        }

        // For a given argument type (types in obj.parentArgument.AcceptableArgumentTypes) this will return all the RuleComponents
        // that return that type or a subtype of that type, as well as any other appropriate values.
        public virtual Dictionary<string, List<RuleComponent>> GetSelectionOptions(RuleComponent obj, ArgumentAccessor parentAccessor, Rule containingRule, RuleType RuleType) {
            Dictionary<string, List<RuleComponent>> AcceptableTypesToReplaceObject = new Dictionary<string, List<RuleComponent>>();

            // Add the types indicated from the parent argument and their associated types
            foreach (Type acceptableType in parentAccessor.GetArgumentObject().AcceptableArgumentTypes) {
                List<RuleComponent> returnTypesAndSubtypes;

                if (!ArgumentTypeToReturnType.TryGetValue(acceptableType, out returnTypesAndSubtypes))
                    Assert.Never("Failed to find associated type list: " + acceptableType.Name);

                List<RuleComponent> list;
                foreach (RuleComponent e in returnTypesAndSubtypes) {

                    if (!AcceptableTypesToReplaceObject.TryGetValue(e.GetSelectionPanelCategory(), out list)) {
                        list = new List<RuleComponent>();
                        AcceptableTypesToReplaceObject.Add(e.GetSelectionPanelCategory(), list);
                    }

                    list.Add(e);

                }

            }

            List<RuleComponent> variableList = new List<RuleComponent>();
            List<Variable> variablesToInclude = null;
            if (SelectableVariables.TryGetValue(RuleType, out variablesToInclude)) {
                foreach (Variable variable in variablesToInclude) {

                    // Check that the variables value is of the right type
                    Type variableType = variable.GetReturnType();
                    bool acceptable = false;
                    foreach (Type acceptableType in parentAccessor.GetArgumentObject().AcceptableArgumentTypes)
                        acceptable = acceptable || TypeFunctions.IsSameOrSubclassOf(variableType, acceptableType);

                    if (acceptable)
                        variableList.Add(variable);

                }
            }

            // Add rule specific variables
            foreach (Variable variable in containingRule.Variables) { 

                // Check that the variables value is of the right type
                Type variableType = variable.GetReturnType();
                bool acceptable = false;
                foreach (Type acceptableType in parentAccessor.GetArgumentObject().AcceptableArgumentTypes)
                    acceptable = acceptable || TypeFunctions.IsSameOrSubclassOf(variableType, acceptableType);

                if (acceptable)
                    variableList.Add(variable);

            }

            // If there are variables, add them to the list
            if (variableList.Count != 0)
                AcceptableTypesToReplaceObject.Add("Variables", variableList);

            toRemove = new Dictionary<string, List<RuleComponent>>();
            foreach (string str in AcceptableTypesToReplaceObject.Keys)
                toRemove.Add(str, new List<RuleComponent>());

            // Apply exclusions
            List<Type> exclusionTypeList = null;
            if (SelectionOptionExclusions.TryGetValue(RuleType, out exclusionTypeList)) {
                foreach (Type type in exclusionTypeList) {
                    MarkObjectsForRemovalOfType(type, AcceptableTypesToReplaceObject);
                }
            }

            // Must do this to avoid editing the list that we're iterating through
            foreach (string str in AcceptableTypesToReplaceObject.Keys)
                foreach (RuleComponent o in toRemove[str])
                    AcceptableTypesToReplaceObject[str].Remove(o);

            return AcceptableTypesToReplaceObject;
        }

        protected Dictionary<string, List<RuleComponent>> toRemove;
        protected virtual void MarkObjectsForRemovalOfType<T>(Dictionary<string, List<RuleComponent>> dict) {
            foreach (string str in dict.Keys)
                foreach (RuleComponent obj in dict[str])
                    if (obj.GetType() == typeof(T))
                        toRemove[str].Add(obj);
        }
        protected virtual void MarkObjectsForRemovalOfType(Type excludedType, Dictionary<string, List<RuleComponent>> dict) {
            foreach (string str in dict.Keys)
                foreach (RuleComponent obj in dict[str])
                    if (obj.GetType() == excludedType)
                        toRemove[str].Add(obj);
        }

        protected virtual void IterateThroughAllAcceptableArgumentTypes(Action<Type> callback) {
            foreach (RuleComponent obj in SelectableObjects)
                IterateThroughAcceptableArgumentTypes(obj, callback);
        }

        protected virtual void IterateThroughAcceptableArgumentTypes(RuleComponent obj, Action<Type> callback) {
            foreach (Argument arg in obj.ArgumentList.argsByOrder) {
                foreach (Type type in arg.AcceptableArgumentTypes) {
                    callback.Invoke(type);
                }
            }
        }

        public virtual void RunIntegrityChecks() {
            Assert.False("There is no RuleComponent that returns a Block value (All statements should return Void)", ArgumentTypeToReturnType.ContainsKey(typeof(Block)));

            foreach (Type type in ArgumentTypeToReturnType.Keys) {
                if (ArgumentTypeToReturnType[type].Count == 0) {
                    Debug.LogWarning("There is no engine object that returns the argument type '" + type.Name + "' or any of its subclasses");
                }
            }
        }

    }

}