using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using RuleEngine;
using RuleEngineAddons.TurnBased;

namespace RuleEngineAddons.RulePanel {

	public class PanelManager : IManager {
        
        protected Vector3 scale1 = new Vector3(1, 1, 1);
		protected Vector2 posHalf = new Vector2(0.5f, 0.5f);

        // State machine
        public StateMachine<GUIEvent, GUIState> GUIStateMachine;

        public List<ButtonAttachment> Buttons;

        public BoardEventable UnitDraggerDropper { get; private set; }
        public GUIEventable RuleObjectDraggerDropper { get; private set; }

        protected RulePanelController RulePanelController;
        protected SelectionPanelManager SelectionPanelManager;

        public override void Preinit() {
            RulePanelController = E.GetController<RulePanelController>();

            Buttons = new List<ButtonAttachment>();

            TreatTemplates();

        }

        public override void Init() {

            SelectionPanelManager = new SelectionPanelManager();
            SelectionPanelManager.SetEngine(E);

            UnitDraggerDropper = new BoardEventable(E);
            RuleObjectDraggerDropper = new GUIEventable(E);

            CreateBackdrop();

        }

        protected virtual void CreateBackdrop() {

            RulePanelController RulePanelController = E.GetController<RulePanelController>();

            GameObject backdrop = new GameObject("Backdrop");

            RectTransform rectTransform = backdrop.AddComponent<RectTransform>();
            rectTransform.SetParent(RulePanelController.OrganisationalObjects.PanelsContainer.transform);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.localPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            backdrop.AddComponent<Image>().color = new Color(0, 0, 0, 0.2f);

            backdrop.SetActive(false);

            RulePanelController.OrganisationalObjects.BackdropPanel = backdrop;

            backdrop.AddComponent<Button>().onClick.AddListener(() => GUIStateMachine.IssueCommand(GUIEvent.Cancel_Selection));

        }

        protected virtual void TreatTemplates() {
            Vector2 ZeroOne = new Vector2(0, 1);

            RulePanelController.GUITemplates.textTemplate.rectTransform.anchorMin = ZeroOne;
            RulePanelController.GUITemplates.textTemplate.rectTransform.anchorMax = ZeroOne;
            RulePanelController.GUITemplates.textTemplate.rectTransform.pivot = ZeroOne;

            ((RectTransform) RulePanelController.GUITemplates.ContainerTemplate.transform).anchorMin = ZeroOne;
            ((RectTransform) RulePanelController.GUITemplates.ContainerTemplate.transform).anchorMax = ZeroOne;
            ((RectTransform) RulePanelController.GUITemplates.ContainerTemplate.transform).pivot = ZeroOne;

        }

        public virtual GameObject NewInstance(GameObject template, GameObject parent) {
			GameObject newInstance = GameObject.Instantiate(template);
			newInstance.transform.SetParent(parent.transform);
			newInstance.transform.localScale = scale1;
			return newInstance;
		}

        public virtual GameObject NewPanelInstance(GameObject panelTemplate, GameObject parent) {

			// Crate the new panel instance
			GameObject newPanel = NewInstance(panelTemplate, parent);
            ConfigurePanelInstance(newPanel);

			return newPanel;
		}

        public virtual PanelDrawingAttachment ConfigurePanelInstance(GameObject panel) {

            // Add a panel drawing attachment, configure it and reset it
            PanelDrawingAttachment attachment = panel.GetComponent<PanelDrawingAttachment>();
            if (attachment == null)
                attachment = panel.AddComponent<PanelDrawingAttachment>();
            ConfigurePanelDrawingAttachment(attachment);
            attachment.Reset();

            return attachment;
        }

        public virtual void SetPanelAsFullscreen(GameObject panel, Canvas canvas) {
			Rect canvasRect = canvas.pixelRect;
			RectTransform panelRect = GetRectTransform(panel);

			panelRect.anchorMin = Vector2.zero;
			panelRect.anchorMax = new Vector2(1,1);
			panelRect.localScale = scale1;
			panelRect.localPosition = new Vector2(-canvasRect.width/2, canvasRect.height/2);
			panelRect.sizeDelta = Vector2.zero;

			PanelDrawingAttachment attachment = GetPanelAttachment(panel);
			if (attachment != null)
				attachment.PanelWidth = canvasRect.width;
		}

        public virtual void SetChildPanelToFillPanel(GameObject panelToFit, GameObject originalPanel) {
            RectTransform originalTransform = GetRectTransform(originalPanel);
            Rect originalRect = originalTransform.rect;
            RectTransform panelToFitRect = GetRectTransform(panelToFit);

            panelToFitRect.anchorMin = Vector2.zero;
            panelToFitRect.anchorMax = new Vector2(1, 1);
            panelToFitRect.pivot = Vector2.up;
            panelToFitRect.localScale = scale1;
            panelToFitRect.position = originalTransform.position;
            panelToFitRect.sizeDelta = Vector2.zero;

            PanelDrawingAttachment attachment = GetPanelAttachment(panelToFit);
            if (attachment != null)
                attachment.PanelWidth = originalRect.width;
        }

        public virtual void SetPanelAsCenteredBestFit(GameObject panel, Canvas canvas) {
			RectTransform panelRect = E.GetManager<PanelManager>().GetRectTransform(panel);

			GridLayoutGroup VLG = panel.GetComponent<GridLayoutGroup>();
			if (VLG == null)
				VLG = panel.AddComponent<GridLayoutGroup>();

			VLG.padding = new RectOffset(10, 10, 10, 10);
			VLG.spacing = new Vector2(10, 10);
			VLG.cellSize = new Vector2(150, 30);
			VLG.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			VLG.constraintCount = 0;

			float height;

			do  {
				VLG.constraintCount++;
				height = (float) Math.Ceiling((double) panel.transform.childCount / VLG.constraintCount) * 40 + 10;
			} while (height > canvas.pixelRect.height * 0.9);

			panelRect.sizeDelta = new Vector2(VLG.constraintCount * 170, height);

			// Center
			panelRect.anchorMin = posHalf;
			panelRect.anchorMax = posHalf;
			panelRect.pivot = posHalf;
			panelRect.localScale = scale1;
			panelRect.localPosition = Vector3.zero;

		}

        public virtual void SetPanelVisibility(GameObject panel, bool visible) {
			panel.SetActive(visible);
		}

        public virtual void SetInFront(GameObject panel) {
			panel.transform.SetAsLastSibling();
		}

        public virtual void SetAsPopUpWithBackdrop(GameObject panel, GameObject backdropPanel) {
			Assert.NotNull("Panel", panel);
			SetPanelVisibility(backdropPanel, true);
			SetInFront(backdropPanel);
			SetPanelVisibility(panel, true);
			SetInFront(panel);
		}

        public virtual PanelDrawingAttachment GetPanelAttachment(GameObject panel) {
			PanelDrawingAttachment attachment = panel.GetComponent<PanelDrawingAttachment>();
			return attachment;
		}

        public virtual void ResetPanel(GameObject panel) {
			PanelDrawingAttachment attachment = GetPanelAttachment(panel);

			DeleteContents(panel.transform);
			attachment.Reset();
		}

        public virtual Dictionary<string, object> WriteTextToPanel(PanelDrawingAttachment panel, GameObject TextTemplate, string text, GameObject parent, GUIEvent? command = null, bool transparentBackground = false, Rule associatedRule = null, RuleComponent associatedRuleComponent = null, ArgumentAccessor associatedRuleComponentAccessor = null) {

			// Set up the text object with the container as parent
			GameObject newText = NewInstance(TextTemplate, parent);

			Text textComponent = newText.GetComponent<Text>();
			Assert.NotNull("textComponent", textComponent);

			ContentSizeFitterMod CSFm = newText.GetComponent<ContentSizeFitterMod>();
			if (CSFm == null)
				CSFm = newText.AddComponent<ContentSizeFitterMod>();

			// Set text on component and on gameobject for editor
			textComponent.text = text;
			newText.name = text;
			
			// Refresh size of the text element
			CSFm.SetLayoutHorizontal();
			CSFm.SetLayoutVertical();
			
			// Get the dimensions of the text element
			Rect textRect = CSFm.GetRect();
			Assert.NotNull("CSFm.GetRect()", textRect);

			if (command.HasValue) {

				// Set up container object for the text object
				GameObject containerObject = NewInstance(panel.ContainerTemplate, parent);
				RectTransform containerRect = GetRectTransform(containerObject);

				newText.transform.SetParent(containerRect);

				// set container and text positions and size of container
				containerRect.sizeDelta = new Vector2(textRect.width + 2 * panel.TextPadding, 
				                                      Math.Min(panel.HalfMaximumLineHeight*2, textRect.height + 2*panel.VerticalLineSpacing));
				
				newText.transform.localPosition = new Vector3(panel.TextPadding, 
				                                              Math.Min(textRect.height/2 - containerRect.rect.height/2, panel.VerticalLineSpacing), 
				                                              0);
				
				containerRect.localPosition = GetLatestInlinePositon(panel, containerRect.rect);

				return MakeGameObjectClickable(containerObject, command.Value, transparentBackground, associatedRule, associatedRuleComponent, associatedRuleComponentAccessor);
			} else {
				newText.transform.localPosition = GetLatestInlinePositon(panel, textRect);
			}

			return null;
		}

        public virtual void WriteNewLineToPanel(PanelDrawingAttachment panel, bool IsMidLine, float extraSpacing = 0.0f) {
			panel.HorizontalOffset = panel.PanelPadding + panel.indent;
			if (panel.MaximumHeightOfCurrentLine != 0) {
				if (IsMidLine) {
					panel.VerticalOffset += panel.MaximumHeightOfCurrentLine + extraSpacing;
				} else
					panel.VerticalOffset += panel.VerticalLineSpacing + panel.MaximumHeightOfCurrentLine + extraSpacing;
			} else {
				panel.VerticalOffset += extraSpacing;
			}
			panel.MaximumHeightOfCurrentLine = 0;
		}

        public virtual void WriteHorizontalSpaceToPanel(PanelDrawingAttachment panel, GameObject obj) {
			GetLatestInlinePositon(panel, GetRectTransform(obj).rect);
		}

        public virtual void WriteHorizontalSpaceToPanel(PanelDrawingAttachment panel, float distance) {
			GetLatestInlinePositon(panel, new Rect(Vector2.zero, new Vector2(distance, 0)));
		}

        public virtual void EnterBlock(PanelDrawingAttachment panel) {
			panel.indent += panel.StandardIndent;
		}

        public virtual void ExitBlock(PanelDrawingAttachment panel) {
			panel.indent -= panel.StandardIndent;
			Assert.True("Indent is not less than zero after exitting block", panel.indent >= 0);
		}

        public virtual Dictionary<string, object> WriteButtonToPanel(PanelDrawingAttachment panel, GameObject ButtonTemplate, GUIEvent command, Rule associatedRule, RuleComponent associatedRuleComponent, ArgumentAccessor associatedRuleComponentAccessor, string Text = null) {
			GameObject newButton = NewInstance(ButtonTemplate, panel.ContainerStack.Peek());

			newButton.transform.localPosition = GetLatestInlinePositon(panel, GetRectTransform(newButton).rect);

			return MakeGameObjectClickable(newButton, command, false, associatedRule, associatedRuleComponent, associatedRuleComponentAccessor);
		}

        public virtual void WriteGameObjectToPanel() {
			
		}

        public virtual RectTransform GetRectTransform(GameObject obj) {
			RectTransform rect = obj.transform as RectTransform;
			if (rect == null)
				throw new Exception("Object does not have RectTransform");
			return rect;
		}

        public virtual void DeleteContents(Transform transform) {
			foreach (Transform t in transform) {
				GameObject.Destroy(t.gameObject);
			}
		}

        protected virtual Vector3 GetLatestInlinePositon(PanelDrawingAttachment panel, Rect newObjectRect) {
			Vector3 inlinePosition;

			// Update maximum height of current line if necessary
			if (newObjectRect.height > panel.MaximumHeightOfCurrentLine) 
				panel.MaximumHeightOfCurrentLine = newObjectRect.height;
			
			// Deal with panel overfow
			if (panel.HorizontalOffset + newObjectRect.width > panel.PanelWidth - panel.PanelPadding)
				WriteNewLineToPanel(panel, true);

			// Calculate the vertical offset
			float vertical = panel.VerticalOffset + panel.HalfMaximumLineHeight - newObjectRect.height/2;
			
			// Calculate the inline position of the new element
			inlinePosition = new Vector3(panel.HorizontalOffset, -vertical, 0);

			// Update the horizontal offset
			panel.HorizontalOffset += newObjectRect.width + panel.TextPadding;

			return inlinePosition;
		}

        public virtual void UpdatePanelHeight(PanelDrawingAttachment panel, float extra = 0) {
			GetRectTransform(panel.gameObject).sizeDelta = new Vector2(panel.GetCanvas().pixelRect.width, 
			                                                  Math.Max(panel.VerticalOffset + panel.PanelPadding + panel.MaximumHeightOfCurrentLine + extra, 0));
		}

        public virtual void RenderRules(RuleRenderingExecutor RuleRenderingExecutor) {
            SelectiveDebug.LogRuleSet("RenderRules - Synchronous");

            SelectiveDebug.LogRuleExecutor("Starting RenderRules");
            SelectiveDebug.StartTimer("RenderRules");

            RuleRenderingExecutor.RuleSorter = "ByUnitType";
            RuleRenderingExecutor.ExecuteRules();
            
            SelectiveDebug.StopTimer("RenderRules");
            SelectiveDebug.LogRuleExecutor("Finished RenderRules");
            
        }

        public virtual void RenderSingleRule(RuleRenderingExecutor RuleRenderingExecutor, RuleType RuleType, int index, GameObject panel, bool lastRuleInType, bool CurrentRuleInaccessible) {
            SelectiveDebug.LogRuleSet("RenderSingleRule - Synchronous");

            SelectiveDebug.LogRuleExecutor("Starting RenderSingleRule");
            SelectiveDebug.StartTimer("RenderSingleRule");

            RuleRenderingExecutor.CurrentRuleInaccessible = CurrentRuleInaccessible;

            RuleRenderingExecutor.ExecuteRule(RuleType, index, lastRuleInType);

            SelectiveDebug.StopTimer("RenderSingleRule");
            SelectiveDebug.LogRuleExecutor("Finished RenderSingleRule");

        }

        public virtual void CloseSelection() {
            SelectionPanelManager.TurnOffSelectionPanel();
        }

        public virtual void OpenSelectionForObject(RuleComponent EditingObject, ArgumentAccessor EditingObjectAccessor, Rule currentRule, RuleType mode) {
            SelectionPanelSetFactory panelSetFactory = new SelectionPanelSetFactory(E, EditingObject, EditingObjectAccessor, currentRule, mode);
            SelectionPanelSet panelSet = panelSetFactory.Create(currentRule);
            SelectionPanelManager.DisplaySelectionPanelSetIndex(panelSet);
        }

        public virtual void CategorySelected(string category) {
            SelectionPanelManager.DisplaySelectionPanelSetCategory(category);
        }

        public virtual Dictionary<string, object> MakeGameObjectClickable(GameObject control, GUIEvent command, bool transparentBackground, Rule associatedRule, RuleComponent associatedObject, ArgumentAccessor associatedObjectAccessor) {

            Button button = control.GetComponent<Button>();

            if (button == null)
                button = control.AddComponent<Button>();

            ButtonAttachment attachment = control.GetComponent<ButtonAttachment>();

            if (attachment == null)
                attachment = control.AddComponent<ButtonAttachment>();

            // Add the appropriate information so that when the control is activated it knows what it needs to do
            attachment.command = command;
            attachment.associatedRule = associatedRule;
            attachment.associatedRuleComponent = associatedObject;
            attachment.associatedRuleComponentAccessor = associatedObjectAccessor;

            // Make the control clickable
            button.onClick.AddListener(() => attachment.ButtonClickHandler());

            // Register this button with the GUIController
            if (associatedObject != null)
                Buttons.Add(attachment);

            // Make the button background transparent if required
            if (transparentBackground)
                button.GetComponent<Image>().color = Color.clear;
            
            return attachment.properties;
        }

        public virtual GameObject GetRuleRenderingPanel() {
            return RulePanelController.OrganisationalObjects.RulePanel;
        }

        public virtual void ConfigurePanelDrawingAttachment(PanelDrawingAttachment attachment) {

            // Find templates
            attachment.TitleTextTemplate = RulePanelController.GUITemplates.titleTextTemplate.gameObject;
            attachment.TextTemplate = RulePanelController.GUITemplates.textTemplate.gameObject;
            attachment.TextPlacemarkerTemplate = RulePanelController.GUITemplates.textPlacemarkerTemplate.gameObject;
            attachment.AddButtonTemplate = RulePanelController.GUITemplates.addButtonTemplate.gameObject;
            attachment.RemoveButtonTemplate = RulePanelController.GUITemplates.removeButtonTemplate.gameObject;
            attachment.ContainerTemplate = RulePanelController.GUITemplates.ContainerTemplate.gameObject;

            // Find other values
            attachment.StandardIndent = RulePanelController.RulePanelDrawingBehaviourSettings.StandardIndent;

        }

        public virtual void SetJudgementText(string text) {
            RulePanelController.RuleJudgeObjects.JudgementText = text;
        }

        public virtual void ShowAddRuleAvailableNotification(bool show) {
            if (RulePanelController.RuleJudgeObjects.AddRule != null)
                RulePanelController.RuleJudgeObjects.AddRule.Available = show;
        }

        public virtual void ShowRemoveRuleAvailableNotification(bool show) {
            if (RulePanelController.RuleJudgeObjects.RemoveRule != null)
                RulePanelController.RuleJudgeObjects.RemoveRule.Available = show;
        }

        public virtual void ShowAddObjectAvailableNotification(bool show) {
            if (RulePanelController.RuleJudgeObjects.AddStatement != null)
                RulePanelController.RuleJudgeObjects.AddStatement.Available = show;
        }

        public virtual void ShowRemoveObjectAvailableNotification(bool show) {
            if (RulePanelController.RuleJudgeObjects.RemoveObject != null)
                RulePanelController.RuleJudgeObjects.RemoveObject.Available = show;
        }

        public virtual void ShowSwapObjectAvailableNotification(bool show) {
            if (RulePanelController.RuleJudgeObjects.SwapObjects != null)
                RulePanelController.RuleJudgeObjects.SwapObjects.Available = show;
        }

        public virtual void ShowReplaceObjectAvailableNotification(bool show) {
            if (RulePanelController.RuleJudgeObjects.ReplaceObject != null)
                RulePanelController.RuleJudgeObjects.ReplaceObject.Available = show;
        }

        public virtual void ShowPossibleDropZonesForObject(RuleComponent obj, ArgumentAccessor objAccessor) {
            foreach (ButtonAttachment attachment in Buttons) {
                if (RuleObjectDraggerDropper.IsMovePermitted(obj, objAccessor, attachment.associatedRuleComponent, attachment.associatedRuleComponentAccessor))
                    attachment.PossibleAction();
            }
        }

        public virtual void UnshowPossibleDropZones() {
            foreach (ButtonAttachment attachment in Buttons) {
                attachment.UnpossibleAction();
            }
        }

    }

}