using System;
using RuleEngine;
using System.Collections.Generic;
using UnityEngine;
using RuleEngineAddons.TurnBased;
using UnityEngine.UI;

namespace RuleEngineAddons.RulePanel {

    public class ActionStackManager : IManager {

        public GameObject ActionStackContainer;
        public RectTransform ActionStackContainerRectTransform;

        public float ItemSpacing = 5f;
        public Vector2 AnchorTo = Vector2.right;
        public float ItemWidth;
        public float ItemHeight;

        public Dictionary<string, ActionStackProfile> Stacks { get; private set; }

        public ActionStackProfile VisibleStack;
        public ActionStackProfile SwapStack;
        public ActionStackProfile ReserveStack;

        public PanelManager PanelManager;
        public RulePanelManager RulePanelManager;
        public ModificationManager ModificationManager;
        public RulePanelController RulePanelController;
        public TurnManager TurnManager;

        protected ClearStackModification OlderClearStackModification;

        public override void Preinit() {
            
        }

        public override void Init() {

            TurnManager = E.GetManager<TurnManager>();
            PanelManager = E.GetManager<PanelManager>();
            RulePanelManager = E.GetManager<RulePanelManager>();
            ModificationManager = E.GetManager<ModificationManager>();
            RulePanelController = E.GetController<RulePanelController>();

            Stacks = new Dictionary<string, ActionStackProfile>();

            // Setup the necessary references to the the stack container
            Assert.NotNull("ActionStackController.ActionStack", RulePanelController.OrganisationalObjects.ActionStackContainer);
            ActionStackContainer = RulePanelController.OrganisationalObjects.ActionStackContainer;
            ActionStackContainerRectTransform = ActionStackContainer.GetComponent<RectTransform>();
            if (ActionStackContainerRectTransform == null)
                ActionStackContainerRectTransform = ActionStackContainer.AddComponent<RectTransform>();

            // Register necessary event handles
            if (TurnManager != null) { 
                TurnManager = E.GetManager<TurnManager>();
                TurnManager.OnBeforeTurnChange += BeforeTurnChange;
                TurnManager.OnTurnCancelled += TurnCancelled;
            }

            // Find item height and width
            ItemWidth = ActionStackContainerRectTransform.rect.width;
            ItemHeight = ActionStackContainerRectTransform.rect.height;

            // Create and configure the stack
            VisibleStack = new ActionStackProfile(E, this, "RuleStack", "Visible Stack");

            // Copy the stack and configure it as the swap stack
            SwapStack = new ActionStackProfile(E, this, "GameStack", "Swap Stack", VisibleStack);
            SwapStack.SetActive(false);

            // Create and configure the reserved object stack
            ReserveStack = new ActionStackProfile(E, this, "ReserveStack", "Reserve Stack");
            ReserveStack.SetActive(false);

        }

        public virtual void SetupInitialStackObjects() {

            Stacks["GameStack"].AddItem_TS(new ActionStackItemProfile() {
                ID = "ToggleButton",
                Type = ActionStackItemProfile.StackOptionType.Button,
                Title = "Rule Panel",
                DestroyOnClick = false,
                Permanent = true,
                OnClick = delegate {
                    RulePanelManager.MainRulePanel.Open();
                }
            }, 0);

            Stacks["RuleStack"].AddItem_TS(new ActionStackItemProfile() {
                ID = "ToggleButton",
                Type = ActionStackItemProfile.StackOptionType.Button,
                Title = "Return to game",
                DestroyOnClick = false,
                Permanent = true,
                OnClick = delegate {
                    RulePanelManager.MainRulePanel.Close();
                }
            });

            Stacks["RuleStack"].AddItem_TS(new ActionStackItemProfile() {
                ID = "CancelButton",
                Type = ActionStackItemProfile.StackOptionType.Button,
                Title = "Cancel Changes",
                DestroyOnClick = false,
                Permanent = true
            });

            Stacks["RuleStack"].AddItem_TS(new ActionStackItemProfile() {
                ID = "ReplaceObject",
                Type = ActionStackItemProfile.StackOptionType.Label,
                Title = "Replace Object",
                DestroyOnClick = false,
                Permanent = true
            });

            Stacks["RuleStack"].AddItem_TS(new ActionStackItemProfile() {
                ID = "SwapObjects",
                Type = ActionStackItemProfile.StackOptionType.Label,
                Title = "Swap Objects",
                DestroyOnClick = false,
                Permanent = true
            });

            Stacks["RuleStack"].AddItem_TS(new ActionStackItemProfile() {
                ID = "RemoveObject",
                Type = ActionStackItemProfile.StackOptionType.Label,
                Title = "Remove Object",
                DestroyOnClick = false,
                Permanent = true
            });

            Stacks["RuleStack"].AddItem_TS(new ActionStackItemProfile() {
                ID = "AddStatement",
                Type = ActionStackItemProfile.StackOptionType.Label,
                Title = "Add Statement",
                DestroyOnClick = false,
                Permanent = true
            });

            Stacks["RuleStack"].AddItem_TS(new ActionStackItemProfile() {
                ID = "RemoveRule",
                Type = ActionStackItemProfile.StackOptionType.Label,
                Title = "Remove Rule",
                DestroyOnClick = false,
                Permanent = true
            });

            Stacks["RuleStack"].AddItem_TS(new ActionStackItemProfile() {
                ID = "AddRule",
                Type = ActionStackItemProfile.StackOptionType.Label,
                Title = "Add Rule",
                DestroyOnClick = false,
                Permanent = true
            });

            Stacks["RuleStack"].AddItem_TS(new ActionStackItemProfile() {
                ID = "Judgement",
                Type = ActionStackItemProfile.StackOptionType.Label,
                Title = "Judgement",
                DestroyOnClick = false,
                Permanent = true
            });

            // Collect the unity object references once they're ready
            new CallbackEffect().Init(FillJudgeObjects).Enqueue();

        }

        public virtual void FillJudgeObjects() {
            RulePanelController.RuleJudgeObjectsContainer R = RulePanelController.RuleJudgeObjects;

            R.CancelButton = Stacks["RuleStack"].GetGameObjectOfItem("CancelButton").GetComponent<Button>();
            R.ReplaceObject = Stacks["RuleStack"].GetGameObjectOfItem("ReplaceObject").AddComponent<JudgeNotificationAttachment>();
            R.SwapObjects = Stacks["RuleStack"].GetGameObjectOfItem("SwapObjects").AddComponent<JudgeNotificationAttachment>();
            R.RemoveObject = Stacks["RuleStack"].GetGameObjectOfItem("RemoveObject").AddComponent<JudgeNotificationAttachment>();
            R.AddStatement = Stacks["RuleStack"].GetGameObjectOfItem("AddStatement").AddComponent<JudgeNotificationAttachment>();
            R.RemoveRule = Stacks["RuleStack"].GetGameObjectOfItem("RemoveRule").AddComponent<JudgeNotificationAttachment>();
            R.AddRule = Stacks["RuleStack"].GetGameObjectOfItem("AddRule").AddComponent<JudgeNotificationAttachment>();

            GameObject JudgementObject = Stacks["RuleStack"].GetGameObjectOfItem("Judgement");
            R.Judgement = JudgementObject.GetComponentInChildren<Text>();

            RecolourJudgementItem(R.ReplaceObject.gameObject);
            RecolourJudgementItem(R.SwapObjects.gameObject);
            RecolourJudgementItem(R.RemoveObject.gameObject);
            RecolourJudgementItem(R.AddStatement.gameObject);
            RecolourJudgementItem(R.RemoveRule.gameObject);
            RecolourJudgementItem(R.AddRule.gameObject);
            RecolourJudgementItem(JudgementObject);

            // Set the state transition of the cancel button on the rule panel
            PanelManager.MakeGameObjectClickable(R.CancelButton.gameObject, GUIEvent.Cancel_Changes, false, null, null, null);

            // Set the state transition of the toggle rule panel buttons
            GameObject RuleStackToggleButton = Stacks["RuleStack"].GetGameObjectOfItem("ToggleButton");
            GameObject GameStackToggleButton = Stacks["GameStack"].GetGameObjectOfItem("ToggleButton");
            
            // Give the object hover notifications
            R.CancelButton.gameObject.AddComponent<HoverNotificationAttachment>().Tooltip = "Click this to undo all changes made to the rules this turn";
            R.ReplaceObject.gameObject.AddComponent<HoverNotificationAttachment>().Tooltip = "Whether the player may replace one object with another";
            R.SwapObjects.gameObject.AddComponent<HoverNotificationAttachment>().Tooltip = "Whether the player may swap two objects";
            R.RemoveObject.gameObject.AddComponent<HoverNotificationAttachment>().Tooltip = "Whether the player may remove an object";
            R.AddStatement.gameObject.AddComponent<HoverNotificationAttachment>().Tooltip = "Whether the player may add a new statement";
            R.RemoveRule.gameObject.AddComponent<HoverNotificationAttachment>().Tooltip = "Whether the player may remove a rule";
            R.AddRule.gameObject.AddComponent<HoverNotificationAttachment>().Tooltip = "Whether the player may add a rule";
            JudgementObject.AddComponent<HoverNotificationAttachment>().Tooltip = "An indication of how much further the player may modify the rule";
            RuleStackToggleButton.AddComponent<HoverNotificationAttachment>().Tooltip = "Switch to the game";
            GameStackToggleButton.AddComponent<HoverNotificationAttachment>().Tooltip = "Switch to the rule panel";

        }

        public virtual void RecolourJudgementItem(GameObject obj) {
            Image image = obj.GetComponent<Image>();
            if (image != null) {
                JudgeNotificationAttachment attachment = obj.GetComponent<JudgeNotificationAttachment>();
                if (attachment != null)
                    image.color = new Color(0, 0, 0, 0.1f);
                else
                    image.color = new Color(0, 0, 0, 0.2f);
            }
        }

        public virtual void SwapStacks() {
            SelectiveDebug.LogActionStack("SwapStacks");

            // Exchange the Stack and SwapStack
            ActionStackProfile tmp = VisibleStack;
            VisibleStack = SwapStack;
            SwapStack = tmp;

            VisibleStack.SetActive(true);
            SwapStack.SetActive(false);

            VisibleStack.SetEditorName("Visible Stack");
            SwapStack.SetEditorName("Swap Stack");

            VisibleStack.UpdateStackDimensions();
        }

        public virtual void BeforeTurnChange() {
            SelectiveDebug.LogActionStack("BeforeTurnChange");

            // Clear the stack before the turn rules are run, so that they can add to the stack for the next player
            OlderClearStackModification = new ClearStackModification(E, "RuleStack");
            ModificationManager.ApplyModification_TS(OlderClearStackModification);
            ModificationManager.ApplyModification_TS(new ClearStackModification(E, "GameStack"));

        }

        public virtual void TurnCancelled() {
            SelectiveDebug.LogActionStack("TurnCancelled");

            ModificationManager.UndoModificationsUpToAndIncluding_TS(OlderClearStackModification);

        }

    }

}