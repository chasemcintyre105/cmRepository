using RuleEngine;
using RuleEngineAddons.TurnBased;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {

	public class GUITransitionHandler : TransitionHandler<GUIEvent, GUIState> {

        private Rule CurrentRule;
        private RuleComponent EditingObject;
		private ArgumentAccessor EditingObjectAccessor;

        public TurnManager TurnManager;
        public PanelManager PanelManager;
        public RulePanelManager RulePanelManager;
        public ActionStackManager ActionStackManager;
        public NotificationManager NotificationManager;
        public StateMachine<GUIEvent, GUIState> GUIStateMachine;

        public GUITransitionHandler(Engine E) : base(E) {
        }

        public override void Init() {
            TurnManager = E.GetManager<TurnManager>();
            PanelManager = E.GetManager<PanelManager>();
            GUIStateMachine = PanelManager.GUIStateMachine;
            RulePanelManager = E.GetManager<RulePanelManager>();
            ActionStackManager = E.GetManager<ActionStackManager>();
            NotificationManager = E.GetManager<NotificationManager>();
        }

        public void Handle_View_Game(Dictionary<string, object> properties) {

            // Move the panel off screen
            RulePanelManager.RulePanelMovable.transform.localPosition += new Vector3(-E.GetManager<PanelManager>().GetRectTransform(RulePanelManager.RulePanelMovable).rect.width, 0, 0);

            // Hide any tooltip that might've been displayed
            NotificationManager.ClearNotifications(); // Ok to do in main thread

            // Change to the game stack
            ActionStackManager.SwapStacks();

            // Tell the main rule panel to close
            RulePanelManager.MainRulePanel.ForceClose();

            E.EffectFactory.EnqueueNewEffect<IClosingRulePanelEffect>();

        }

        public void Handle_View_Rules(Dictionary<string, object> properties) {

            object MoveCancelled = false;
            if (properties != null)
                properties.TryGetValue("MoveCancelled", out MoveCancelled);

            // Check whether the turn was cancelled after closing the rule panel, and so keep the panel open
            if ((bool) MoveCancelled && RulePanelManager.MainRulePanel.IsOpen)
                return;

            // If it's the first time the panel is opened, refresh it
            if (RulePanelManager.firstOpeningOfRulePanel) {
                RulePanelManager.RefreshMainRulePanel();
                RulePanelManager.firstOpeningOfRulePanel = false;
            }

            // Cancel any dragging going on
            PanelManager.UnitDraggerDropper.CancelDragging();

            // Slide the panel into view
            RulePanelManager.RulePanelMovable.transform.localPosition += new Vector3(E.GetManager<PanelManager>().GetRectTransform(RulePanelManager.RulePanelMovable).rect.width, 0, 0);

            // Hide any tooltip that might've been displayed
            NotificationManager.ClearNotifications();

            // Change to the rule panel stack
            ActionStackManager.SwapStacks();

            // Tell the main rule panel to open
            RulePanelManager.MainRulePanel.ForceOpen();

            E.EffectFactory.EnqueueNewEffect<IOpeningRulePanelEffect>();

        }

        public void Handle_Check_Rules(Dictionary<string, object> properties) {
            
			switch (E.RuleManager.RuleJudge.GetJudgement()) {
			case Judgement.NothingChanged:
				// The rule judge has determined that the player has not modified the rules

				// Return to the game without doing anything else
				GUIStateMachine.IssueCommand(GUIEvent.View_Game);

				break;
			case Judgement.StillGoing:
				// The rule judge has determined that the player has modified the rules and has not completed a turn

				// Cancel any changes made to the rules
				CancelChangesToRules();

                // Rerendering the rules is necessary after cancelling changes
                RulePanelManager.RefreshMainRulePanel();

                // Go back to view the rules so that the player can see that the changes have been cancelled
                Dictionary<string, object> newProperties = new Dictionary<string, object>();
                newProperties.Add("MoveCancelled", true);
                GUIStateMachine.IssueCommand(GUIEvent.View_Rules, newProperties);

				break;
			case Judgement.Finished:
                // The rule judge has determined that the player has modified the rules and completed a turn

                CheckAndChangeTurn();

                break;
			}

		}

        private void CheckAndChangeTurn() {

            // Verify the rule, update the possible actions, and the switch the players
            E.ExecutionManager.VerifyRulesAsynchronously(delegate (RuleVerifyingExecutor RuleExecutor) {

                // Reset current rule properties, if necessary
                if (CurrentRule != null)
                    CurrentRule.SomeElementsEditable = false;

                if (RuleExecutor.MustRevertToPreviousRuleSet) {

                    E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("Error", "Errors were found when verifying new rule set. Changes are being reverted.");

                    // Cancel the changes and refresh the rule panel
                    CancelChangesToRules();
                    RulePanelManager.RefreshMainRulePanel();

                    // Go back to view the rules so that the player can see that the changes have been cancelled
                    GUIStateMachine.IssueCommand(GUIEvent.View_Rules);

                } else {

                    E.ExecutionManager.CompileRulesAsynchronously(delegate {

                        // Switch players (Note that this updates the possible actions)
                        GUIStateMachine.IssueCommand(GUIEvent.Switch_Players);

                    });

                }

            });

        }

		public void Handle_Add_Rule(Dictionary<string, object> properties) {

			AddRuleModification mod;
			RuleType mode = GetProperty<RuleType>(properties, "RuleType");
            
            mod = new AddRuleModification(E, E.RuleManager.RuleTypeToList[mode]);

			E.RuleManager.ModStack.ApplyModification(mod);

            CurrentRule = mod.AssociatedRule;
            CurrentRule.SomeElementsEditable = true;

            RulePanelManager.RefreshMainRulePanel();

		}

		public void Handle_Remove_Rule(Dictionary<string, object> properties) {

            // Get property ruleIndex
            CurrentRule = GetProperty<Rule>(properties, "Rule");
            int ruleIndex = GetProperty<int>(properties, "RuleIndex");
			RuleType RuleType = GetProperty<RuleType>(properties, "RuleType");

			E.RuleManager.ModStack.ApplyModification(new RemoveRuleModification(CurrentRule, E.RuleManager.RuleTypeToList[RuleType], ruleIndex));

            RulePanelManager.RefreshMainRulePanel();

		}
		
		public void Handle_Add_Statement(Dictionary<string, object> properties) {

            // Get property ruleIndex
            CurrentRule = GetProperty<Rule>(properties, "Rule");
			Block block = GetProperty<Block>(properties, "Block");
            block.Editability = RuleComponent.RuleComponentEditability.Editable;
            CurrentRule.SomeElementsEditable = true;

            E.RuleManager.ModStack.ApplyModification(new AddStatementModification(CurrentRule, block));

            RulePanelManager.RefreshMainRulePanel();

		}

		public void Handle_Remove_Object(Dictionary<string, object> properties) {

            CurrentRule = GetProperty<Rule>(properties, "Rule");

            // Remove the statement or value
            E.RuleManager.ModStack.ApplyModification(new RemoveRuleComponentModification(CurrentRule, EditingObject, EditingObjectAccessor));

            // Close the selection panel and stop editing the objecr
            PanelManager.CloseSelection();
			EditingObject = null;

            // Refresh the rule panel
            RulePanelManager.RefreshMainRulePanel();

		}

		public void Handle_Edit_Object(Dictionary<string, object> properties) {

            // Get objects that allow the game to construct an edit dialog
            EditingObject = GetProperty<RuleComponent>(properties, "Object");
            RuleContext currentRuleContext = GetProperty<RuleContext>(properties, "RuleContext");
            RuleType ruleType = GetProperty<RuleType>(properties, "RuleType");
            EditingObjectAccessor = GetProperty<ArgumentAccessor>(properties, "ParentAccessor");

            PanelManager.OpenSelectionForObject(EditingObject, EditingObjectAccessor, currentRuleContext.Rule, ruleType);

		}
		
		public void Handle_Cancel_Selection(Dictionary<string, object> properties) {

            PanelManager.CloseSelection();
			EditingObject = null;

		}
		
		public void Handle_Select_Category(Dictionary<string, object> properties) {

			string category = GetProperty<string>(properties, "Category");
            PanelManager.CategorySelected(category);

		}

		public void Handle_Select_Object(Dictionary<string, object> properties) {

			RuleComponent NewObjectTemplate = GetProperty<RuleComponent>(properties, "Selection");
			RuleComponent NewObject = MakeCopyOfObject(NewObjectTemplate);

            Assert.NotNull("Handle_Select_Object: CurrentRule", CurrentRule); // TODO Remove

			E.RuleManager.ModStack.ApplyModification(new ReplaceRuleComponentModification(CurrentRule, EditingObject, EditingObjectAccessor, NewObject));

            // Set the old object into the new one as an argument if possible
            // This is a convenience that allows a move like the following:
            //   Replacing IsEqualTo with Not in ...If (IsEqualTo(a, b))... results in ...If (Not(IsEqualTo(a, b)))... rather than ...If (Not( Not Selected ))...
            if (!(EditingObject is VoidStatement)) // Void statements may be ignored
                foreach (Argument arg in NewObject.ArgumentList.argsByOrder) {
				    if (NewObject.ArgumentList.CanBeSetAsArgument(arg.index, EditingObject.NewRef())) {
                        NewObject.ArgumentList.SetArgument(arg.index, EditingObject.NewRef()); 
					    break;
				    }
			    }

            PanelManager.CloseSelection();
            RulePanelManager.RefreshMainRulePanel();

			EditingObject = null;

		}

		public void Handle_Cancel_Changes(Dictionary<string, object> properties) {

			CancelChangesToRules();

            RulePanelManager.RefreshMainRulePanel();

		}
		
		public void Handle_Switch_Players(Dictionary<string, object> properties) {

            if (TurnManager != null) {
                TurnManager.RequestNextTurn((Action<bool>) delegate (bool MoveCancelled) {
                    if (MoveCancelled) {

                        E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("RuleError", "Move cancelled");

                        // Cancel any changes to the rules and refresh the rule panel
                        CancelChangesToRules();
                        RulePanelManager.RefreshMainRulePanel();

                        Dictionary<string, object> newProperties = new Dictionary<string, object>();
                        newProperties.Add("MoveCancelled", true);
                        GUIStateMachine.IssueCommand(GUIEvent.View_Rules, newProperties);

                    } else {

                        // Refresh the rule panel, ready for the next time it is opened
                        RulePanelManager.RefreshMainRulePanel();

                        GUIStateMachine.IssueCommand(GUIEvent.View_Game);

                    }
                });
            } else {

                // Refresh the rule panel, ready for the next time it is opened
                RulePanelManager.RefreshMainRulePanel();

                GUIStateMachine.IssueCommand(GUIEvent.View_Game);

            }

        }
         
        public void Handle_Toggle_Rule_Visible(Dictionary<string, object> properties) {
			GameObject container = GetProperty<GameObject>(properties, "Container");

            container.SetActive(!container.activeSelf);

        }

        private void CancelChangesToRules() {
			E.RuleManager.ModStack.UndoAllModifications();
			E.RuleManager.RuleJudge.ClearJudgement();
		}

		private RuleComponent MakeCopyOfObject(RuleComponent obj) {

			// Copy the object given. Must be done in this order because Expression is a subtype of Value
			if (obj is Expression || obj is Statement) {
				return (RuleComponent) Activator.CreateInstance(obj.GetType(), E);
			} else if (obj is Value) {
				return obj;
			} else {
				throw new Exception("Should not happen: no other engine object types to account for");
			}
		}

	}

}

