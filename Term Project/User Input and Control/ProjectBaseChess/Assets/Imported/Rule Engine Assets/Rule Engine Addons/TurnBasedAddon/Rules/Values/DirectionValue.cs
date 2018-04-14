using System;
using UnityEngine;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class DirectionValue : Value {

		public static Vector3 up_right = Vector3.up + Vector3.right;
		public static Vector3 up_left = Vector3.up - Vector3.right;
        
		public override string GetSelectionPanelCategory() {
			return "Directions";
		}
        
		public override bool IsEqualTo (Value v) {

			if (v.GetType() != typeof(DirectionValue))
				return false;

			return ((DirectionValue) v).value == value;
		}

		public readonly Vector3 value; // Positive y is forward, positive x is left

		public DirectionValue(Engine E, Vector3 value) : base(E) { 
			this.value = value; 
		}

		public DirectionValue(Engine E, Direction value) : base(E) { 
			switch (value) {
			case Direction.Forward:
				this.value = Vector3.up;
				break;
			case Direction.Forward_left:
				this.value = up_left;
				break;
			case Direction.Left:
				this.value = -Vector3.right;
				break;
			case Direction.Backward_left:
				this.value = -up_right;
				break;
			case Direction.Backward:
				this.value = -Vector3.up;
				break;
			case Direction.Backward_right:
				this.value = -up_left;
				break;
			case Direction.Right:
				this.value = Vector3.right;
				break;
			case Direction.Forward_right:
				this.value = up_right;
				break;
			}
		}
		
		public override string GetDescription() {
			return "Represents a relative direction, both to a unit and a player.";
		}

        public override string ToString() {
            string displayName = "";
            if (value == Vector3.zero) {
                displayName += "<no displacement>";
            } else {
                if (value.x > 0) {
                    displayName += ((int) value.x) + " Left";
                } else if (value.x < 0) {
                    displayName += ((int) -value.x) + " Right";
                }
                if (value.y > 0) {
                    if (displayName != "")
                        displayName += ", ";
                    displayName += ((int) value.y) + " Forward";
                } else if (value.y < 0) {
                    if (displayName != "")
                        displayName += ", ";
                    displayName += ((int) -value.y) + " Backward";
                }
                if (value.z > 0) {
                    if (displayName != "")
                        displayName += ", ";
                    displayName += ((int) value.z) + " Upward";
                } else if (value.z < 0) {
                    if (displayName != "")
                        displayName += ", ";
                    displayName += ((int) -value.z) + " Downward";
                }
            }
            return displayName;
        }

    }

}