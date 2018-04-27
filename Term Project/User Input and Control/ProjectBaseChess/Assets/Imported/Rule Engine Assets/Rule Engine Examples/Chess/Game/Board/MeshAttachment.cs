using RuleEngine;
using RuleEngineAddons.RulePanel;
using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class MeshAttachment : IMeshAttachment {

		public IBoardObjectAttachment attachment;
		public bool FollowOnMouseDown = false;
		public bool FollowOnMouseUp = false;

        private TurnController _BoardController;
        private TurnController BoardController {
            get {
                if (_BoardController == null)
                    _BoardController = RuleEngineController.E.GetController<TurnController>();

                return _BoardController;
            }
        }

        private RulePanelManager _RulePanelManager;
        private RulePanelManager RulePanelManager
        {
            get
            {
                if (_RulePanelManager == null)
                    _RulePanelManager = RuleEngineController.E.GetManager<RulePanelManager>();

                return _RulePanelManager;
            }
        }

        private CameraController _CameraController;
        private CameraController CameraController
        {
            get
            {
                if (_CameraController == null)
                    _CameraController = RuleEngineController.E.GetController<CameraController>();

                return _CameraController;
            }
        }

        void OnMouseDown() {
			if (BoardController.GameAcceptingUserInput)
				attachment.OnMouseDown();
		}
		
		void OnMouseUp() {
			if (BoardController.GameAcceptingUserInput)
				attachment.OnMouseUp();
		}

		void OnMouseOver() {
			if (BoardController.GameAcceptingUserInput) {
				if (Input.GetMouseButtonDown(1)) {
					if (FollowOnMouseDown)
						CameraController.cameraMovement.FollowTarget(attachment.GetGameObject().transform);
				} else if (Input.GetMouseButtonUp(1)) {
					if (FollowOnMouseUp)
						CameraController.cameraMovement.FollowTarget(attachment.GetGameObject().transform);
				}
			}
        }

        void OnMouseEnter() {
            if (!RulePanelManager.MainRulePanel.IsOpen) {
                attachment.OnMouseEnter();
            }
        }

        public override IBoardObjectAttachment GetAttachment() {
            return attachment;
        }

    }
}