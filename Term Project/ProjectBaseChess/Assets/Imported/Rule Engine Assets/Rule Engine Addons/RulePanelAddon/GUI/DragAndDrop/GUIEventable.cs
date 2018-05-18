using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class GUIEventable : DragAndDropEventable<ButtonAttachment, ButtonAttachment> {

		private Engine E;
        protected PanelManager PanelManager;
        protected RulePanelManager RulePanelManager;

        public GUIEventable(Engine E) {
			this.E = E;
            PanelManager = E.GetManager<PanelManager>();
            RulePanelManager = E.GetManager<RulePanelManager>();
        }

		protected override bool CanDrag(ButtonAttachment button) {
			return RulePanelManager.MainRulePanel.IsOpen &&
                   PanelManager.GUIStateMachine.CurrentState == GUIState.Viewing_Rules &&
				   button.associatedRuleComponent != null;
		}

		protected override void StartedDragging() {
            PanelManager.ShowPossibleDropZonesForObject(draggable.associatedRuleComponent, draggable.associatedRuleComponentAccessor);
		}

		protected override void StoppedDragging() {
            PanelManager.UnshowPossibleDropZones();
		}

		protected override void Dropped() {

			SelectiveDebug.LogDragDrop("Dropped with BoardEventable: " + draggable.associatedRuleComponent.ToString() + " -> " + droppable.associatedRuleComponent.ToString());

			// Swap the engine objects
			E.RuleManager.ModStack.ApplyModification(new SwapRuleComponentsModification(draggable.associatedRule,
                                                                                        draggable.associatedRuleComponent, 
                                                                                        draggable.associatedRuleComponentAccessor,
                                                                                        droppable.associatedRuleComponent,
                                                                                        droppable.associatedRuleComponentAccessor));

            RulePanelManager.MainRulePanel.Redraw();
		}

		protected override bool IsDropPermitted() {
			return IsMovePermitted(draggable.associatedRuleComponent, 
			                       draggable.associatedRuleComponentAccessor, 
			                       droppable.associatedRuleComponent,
			                       droppable.associatedRuleComponentAccessor);
		}

        public bool IsMovePermitted(RuleComponent Incident, ArgumentAccessor IncidentAccessor, RuleComponent Target, ArgumentAccessor TargetAccessor) {
            Assert.NotNull("Incident object is not null", Incident);

            // If one has not been given an engine object, it means it is a button and should be ignored
            if (Target == null) {
                return false;
            }
            
            return IncidentAccessor.CanBeSetAsArgument(Target) &&
                   TargetAccessor.CanBeSetAsArgument(Incident);
        }

    }

}

