using UnityEngine;

namespace RuleEngineAddons.TurnBased {

	public class PlayerPosition {
        
        public readonly Vector3 PlayerOrigin;
		public readonly bool ExchangeXY;
        public readonly Quaternion rotationAboutZ;
        public readonly Quaternion rotationAboutY;
        public readonly Quaternion undoRotationAboutZ;
        public readonly Quaternion undoRotationAboutY;
        public readonly float AngleFromGlobalForwardXY;
        public readonly float AngleFromGlobalForwardXZ;

        private readonly Vector3 GlobalOrigin;

        public PlayerPosition(Vector3 PlayerOrigin, float AngleFromGlobalForwardXY, float AngleFromGlobalForwardXZ, bool ExchangeXY) {

            this.PlayerOrigin = PlayerOrigin;
			this.ExchangeXY = ExchangeXY;
            this.AngleFromGlobalForwardXY = AngleFromGlobalForwardXY;
            this.AngleFromGlobalForwardXZ = AngleFromGlobalForwardXZ;

            // Create the quaternions that will perform the rotations
            rotationAboutZ = Quaternion.AngleAxis(AngleFromGlobalForwardXY, Vector3.forward);
            rotationAboutY = Quaternion.AngleAxis(AngleFromGlobalForwardXZ, Vector3.up);
            undoRotationAboutZ = Quaternion.AngleAxis(-AngleFromGlobalForwardXY, Vector3.forward);
            undoRotationAboutY = Quaternion.AngleAxis(-AngleFromGlobalForwardXZ, Vector3.up);
            GlobalOrigin = new Vector3(1, 1, 1);

        }

        // A position must be rotated, displaced and mirrored
        public Vector3 GlobalToPlayerRelativePosition(Vector3 position) {

            // Firstly move the origin
            position = position - PlayerOrigin;

            // Then treat it as a direction
			return GlobalToPlayerRelativeDirection(position) + GlobalOrigin;
		}

        // A direction must simply be rotated and mirrored, it does not take into account a player origin
		public Vector3 GlobalToPlayerRelativeDirection(Vector3 direction) {

            // Then do any exchanges
            if (ExchangeXY)
                direction = new Vector3(direction.y, direction.x, direction.z);

            // Lastly perform all rotations
            direction = rotationAboutY * (rotationAboutZ * direction);

            return direction;
        }
        
    }

}