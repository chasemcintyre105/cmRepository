using UnityEngine;
using System;
using System.Collections.Generic;
using RuleEngine;
using RuleEngineAddons.TurnBased;

namespace RuleEngineExamples.Chess {

	public class MoveObjectAttachment : MonoBehaviour {

		public static MoveObjectAttachment GetMoveObject(GameObject go) {
			MoveObjectAttachment mo = go.GetComponent<MoveObjectAttachment>();
			if (mo == null)
				mo = go.AddComponent<MoveObjectAttachment>();

			return mo;
		}

		private class MovementProfile {
			public Unit unit;
			public GameObject destinationPosition;
			public Action callback;
		}

		private Queue<MovementProfile> queue;
		private MovementProfile currentMovement;

		public MoveObjectAttachment() {
			queue = new Queue<MovementProfile>();
			currentMovement = null;
		}

		public void Engage(Unit unit, GameObject destinationPosition, Action callback = null) {
			Assert.NotNull("Unit", unit);
			Assert.NotNull("destinationTile", destinationPosition);

			// Setup movement profile
			MovementProfile movement = new MovementProfile() {
				unit = unit,
				destinationPosition = destinationPosition,
				callback = callback
			};

			if (currentMovement != null) {

				// Add the movement to the queue if there is already a movement in progres
				queue.Enqueue(movement);

			} else {

				// If this is the first movement, start it straight away
				currentMovement = movement;
				ProcessCurrentMovement();

			}

		}

		private void ProcessCurrentMovement() {

            currentMovement.unit.GetGameObject().transform.SetParent(currentMovement.destinationPosition.transform);
            currentMovement.unit.GetGameObject().transform.localPosition = Vector3.zero;

            Callback();

        }

		private void Callback() {

			// Call any movement specific callback if it exists
			if (currentMovement.callback != null)
				currentMovement.callback.Invoke();

            // The current movement is not finished
            currentMovement = null;

            // Destroy this movement component if there are no movements left to perform
            if (queue.Count == 0) {
				Destroy(this); 
			} else {

				// Otherwise take next movement and process it
				currentMovement = queue.Dequeue();
				ProcessCurrentMovement();

			}
		}

	}
	
}