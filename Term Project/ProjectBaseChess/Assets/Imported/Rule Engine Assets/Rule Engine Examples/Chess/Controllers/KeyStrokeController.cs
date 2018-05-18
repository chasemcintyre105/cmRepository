using UnityEngine;
using RuleEngine;
using RuleEngineAddons.TurnBased;
using RuleEngineAddons.RulePanel;

namespace RuleEngineExamples.Chess {

    public class KeyStrokeController : IController {

        private float? RightMouseAdjustment;

        protected GUIManager GUIManager;
        protected TurnManager TurnManager;
        protected PanelManager PanelManager;
        protected RulePanelManager RulePanelManager;
        protected CameraController CameraController;

        public override void Preinit() {
        }

        public override void Init() {
            GUIManager = E.GetManager<GUIManager>();
            TurnManager = E.GetManager<TurnManager>();
            PanelManager = E.GetManager<PanelManager>();
            RulePanelManager = E.GetManager<RulePanelManager>();
            CameraController = RuleEngineController.E.GetController<CameraController>();
        }

        void Update () {

			if (Input.GetKeyDown(KeyCode.Space)) {
                RulePanelManager.ToggleMainRulePanel();
			} else if (Input.GetKeyDown(KeyCode.Escape)) {
                if (TurnManager.TurnStateMachine.CurrentState == TurnState.Placing_Object) {
                    E.EffectFactory.EnqueueNewEffect<IStopGameObjectPlacementEffect>();
                    TurnManager.TurnStateMachine.IssueCommand(TurnEvent.Wait_For_Input);
                }
			}

            if (Input.GetMouseButtonDown(1)) { // Right mouse button down
                RightMouseAdjustment = Input.mousePosition.x;
            } else if (Input.GetMouseButtonUp(1)) { // Right mouse button up
                RightMouseAdjustment = null;
            } else if (RightMouseAdjustment.HasValue) { // Right mouse button held
                CameraController.OffsetRotation(RightMouseAdjustment.Value - Input.mousePosition.x);
                RightMouseAdjustment = Input.mousePosition.x;
            }

            if (Input.mouseScrollDelta != Vector2.zero && !RulePanelManager.MainRulePanel.IsOpen) {
                CameraController.OffsetZoom(Input.mouseScrollDelta.y);
            }
            
		}

	}

}