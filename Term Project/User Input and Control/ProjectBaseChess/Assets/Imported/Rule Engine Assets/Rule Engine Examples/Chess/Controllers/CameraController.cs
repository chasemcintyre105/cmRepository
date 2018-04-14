using RuleEngine;
using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class CameraController : IController {
        
		public CameraMovement cameraMovement;

        public float ZoomSensitivity;
        public float RotationSensitivity;
        public float ZoomAngle;
        public float RotationAngle;
        public float ZoomCircleRadius;
        public Vector3 ZoomCircleBasedRelativeDisplacement;
        
        public override void Preinit() {
            Assert.NotNull("Could not find a camera", cameraMovement);
        }

        public override void Init() {
        }

        public void SetInitialCameraZoom(float angle) {

            ZoomAngle = Mathf.Clamp(angle, -180f, -90f);

            if (cameraMovement.IsActivated)
                cameraMovement.UpdateViewingAngleForPlayer();

        }

        public void SetInitialCameraRotation(float angle) {

            RotationAngle = Mathf.Clamp(angle, -180f, 0f);

            if (cameraMovement.IsActivated)
                cameraMovement.UpdateViewingAngleForPlayer();

        }

        public void SetInitialCameraFocus(Vector3 positionOffset) {

            new CallbackEffect().Init(delegate {

                Position position = E.GetManager<BoardManager>().GetPosition_TS(positionOffset);
                Assert.NotNull("Position not found for initial camera position", position);

                cameraMovement.gameObject.SetActive(true);
                cameraMovement.Activate(E, position.GetGameObject().transform);

            }).Enqueue();
            
        }

        public void OffsetZoom(float adjustment) {

            // Set the new angle and clamp it to [-180, -90]
            ZoomAngle = Mathf.Clamp(ZoomAngle + ZoomSensitivity * adjustment, -180f, -90f);
            cameraMovement.UpdateViewingAngleForPlayer();

        }

        public void OffsetRotation(float adjustment) {

            // Set the new angle and clamp it to [-180, 0]
            RotationAngle = Mathf.Clamp(RotationAngle + RotationSensitivity * adjustment, -180f, 0f);
            cameraMovement.UpdateViewingAngleForPlayer();

        }

    }

}