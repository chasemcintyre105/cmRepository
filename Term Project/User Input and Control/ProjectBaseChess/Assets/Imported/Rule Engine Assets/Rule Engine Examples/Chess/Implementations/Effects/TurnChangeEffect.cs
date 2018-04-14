using RuleEngine;
using RuleEngineAddons.RulePanel;
using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineExamples.Chess {

	public class TurnChangeEffect : ITurnChangeEffect {

        private Player player;

        public override Effect Init(params object[] parameters) {
            player = (Player) parameters[0];
            return this;
        }

        public override void Apply() {

            if (RuleEngineController.E.GetManager<TurnManager>().CurrentTurn.number == 1) {

                // First turn
                RuleEngineController.E.GetController<CameraController>().SetInitialCameraFocus(new Vector3(4f, 4.5f, 0));

            } else {

                float x;

                if (player.UID == 0)
                    x = 4;
                else
                    x = 6;

                Position position = RuleEngineController.E.GetManager<BoardManager>().GetPosition_TS(new Vector3(x, 4.5f, 0));

                RuleEngineController.E.GetController<CameraController>().cameraMovement.FollowTarget(position.GetGameObject().transform);

            }

            RuleEngineController.E.GetManager<NotificationManager>().ClearNotifications();

        }

        public override object[] GetEffectData() {
            return new object[] { };
        }

    }

}