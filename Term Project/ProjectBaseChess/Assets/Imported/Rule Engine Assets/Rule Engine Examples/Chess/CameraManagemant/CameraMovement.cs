using RuleEngine;
using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class CameraMovement : MonoBehaviour {

		public float smooth = 2.5f; // The relative speed at which the camera will catch up.
        public float smoothView = 1.3f; // The relative speed at which the camera will catch up for viewing angle changes
         
        public Transform targetTransform; // Reference to the gameobject to follow
		public Vector3 relCameraPos; // The relative position of the camera from the player.
		public float relCameraPosMag; // The distance of the camera from the player.
        public Vector3 GlobalOffset;

        private GameObject Board;

        private ParametricCircleYZPlane VerticalCurve;
        private ParametricCircleXYPlane HorizontalCurve;

        public bool IsActivated { get; private set; }

        protected TurnManager TurnManager;
        private CameraController CameraController;
        private TurnController BoardController;

        public CameraMovement() {
            IsActivated = false;
        }

		void Awake () {
            gameObject.SetActive(false);
		}

		public void Activate(Engine E, Transform targetObject) {

			if (targetObject == null) {
				gameObject.SetActive(false);
				Assert.Never("targetObject");
			}

            TurnManager = E.GetManager<TurnManager>();
            CameraController = E.GetController<CameraController>();
            BoardController = E.GetController<TurnController>();

            Board = BoardController.GameContainer;
            VerticalCurve = new ParametricCircleYZPlane(Vector3.zero, CameraController.ZoomAngle, CameraController.ZoomCircleRadius);
            HorizontalCurve = new ParametricCircleXYPlane(Vector3.zero, CameraController.RotationAngle, CameraController.ZoomCircleRadius);

            targetTransform = new GameObject("Camera Target").transform;
            targetTransform.SetParent(Board.transform);
            targetTransform.position = targetObject.position;
            
			UpdateViewingAngleForPlayer();

            // Set the initial position of the camera local to the board and make it look at it
            transform.localPosition = targetObject.position + relCameraPos;
			transform.LookAt(targetObject);
            
            // Setup referencing for turn event
            TurnManager.OnTurnChanged += UpdateViewingAngleForPlayer;

            IsActivated = true;

		}

        public void UpdateViewingAngleForPlayer() {

            // Update the relative position of the camera in the scene and the magnitude of this vector.
            relCameraPos = BoardOffsetToGlobalVector(CalculateBoardRelativeCameraPosition());
			relCameraPosMag = relCameraPos.magnitude;

		}

        public Vector3 BoardOffsetToGlobalVector(Vector3 boardLocalVector) {

            // Find the forward direction for the current player 
            Vector3 cameraOffset = TurnManager.CurrentTurn.player.Position.GlobalToPlayerRelativePosition(boardLocalVector + TurnManager.CurrentTurn.player.Position.PlayerOrigin);

            // Transform this into direction local to the board
            return BoardController.BoardScaleModifier * Board.transform.InverseTransformDirection(cameraOffset);

        }

        private Vector3 CalculateBoardRelativeCameraPosition() {
            Vector3 pos = CameraController.ZoomCircleBasedRelativeDisplacement;
            pos += VerticalCurve.CalculatePointOnCurveFromParameter(CameraController.ZoomAngle);

            HorizontalCurve.Radius = Mathf.Sqrt(pos.x * pos.x + pos.y * pos.y);
            pos += HorizontalCurve.CalculatePointOnCurveFromParameter(CameraController.RotationAngle) + new Vector3(0, HorizontalCurve.Radius, 0);

            return pos;
        }
        
        public void FollowTarget(Transform target) {
            targetTransform.position = target.position;
        }

		void FixedUpdate() {
			if (targetTransform == null) {
				return;
			}
            
			// Lerp the camera's position between it's current position and it's new position.
			transform.position = Vector3.Lerp(transform.position, 
                                              targetTransform.position + relCameraPos + GlobalOffset,  
                                              smooth * Time.deltaTime);

            // Make sure the camera is looking at the player.
            SmoothLookAt();

        }

        void SmoothLookAt() {

			// Create a vector from the camera towards the player.
			Vector3 relPlayerPosition = targetTransform.position - transform.position;
			
			// Create a rotation based on the relative position of the player being the forward vector.
			Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
			
			// Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
			transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smoothView * Time.deltaTime);

		}

	}

}