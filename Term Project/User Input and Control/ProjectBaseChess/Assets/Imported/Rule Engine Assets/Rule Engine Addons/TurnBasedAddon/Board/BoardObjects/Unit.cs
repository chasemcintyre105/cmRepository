using UnityEngine;
using System.Collections.Generic;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class Unit : IBoardObject {

		private readonly object _unitLock = new object();

        public readonly int ID;
        public readonly UnitType type;
		public Player player;
		public GameObject template;
        public bool Placemarker;

        private Stack<Vector3> lastOffset;
		private Vector3 _offset;
        private GameObject gameObject;
		private IBoardObjectAttachment Attachment;
		private GameObject AttachmentGameObject;

		private List<PossibleAction> possibleActions;

		public Unit(int ID, UnitType type) {
            this.ID = ID;
            this.type = type;

			_unitLock = new object();

			possibleActions = new List<PossibleAction>();
			lastOffset = new Stack<Vector3>();
            Placemarker = false;

		}

        public int GetID() {
            return ID;
        }

        public IObjectType GetObjectType() {
            return type;
        }

        // Offset functions
        private Vector3 offset {
			get {
				return _offset;
			}
			set {
				lastOffset.Push(_offset);
				_offset = value;
			}
		}

		public Vector3 lastOffset_TS {
			get {
				lock (_unitLock) {
					return lastOffset.Peek();
				}
			}
		}

		public void UndoLastMove_TS() {
			lock (_unitLock) {
				_offset = lastOffset.Pop();
			}
		}

		public void ClearMoves_TS() {
			lock (_unitLock) {
				lastOffset.Clear();
				lastOffset.Push(_offset);
			}
		}
		
		public void SetOffset_TS(Vector3 offset) { 
			lock (_unitLock) {
				this.offset = offset; 
			}
		}
		
		public Vector3 GetOffset_TS() { 
			lock (_unitLock) {
				return offset;
			}
		}

		// Board object implemenations
		public IBoardObjectType GetBoardObjectType() {
			return type;
		}

		public void SetBoardObjectAttachment(IBoardObjectAttachment value) {
			Attachment = value;
		}
		
		public IBoardObjectAttachment GetBoardObjectAttachment() { 
			return Attachment; 
		}

        public void SetGameObject(GameObject gameObject) {
            this.gameObject = gameObject;
        }

        public GameObject GetGameObject() {
			return gameObject; 
		}

		// Thread safe methods
		public IEnumerable<PossibleAction> GetPossibleActions_TS() {
			lock (_unitLock) {
				return possibleActions;
			}
		}

		public void AddPossibleAction_TS(PossibleAction action) {
			lock (_unitLock) {
				possibleActions.Add(action);
			}
		}

		public void ClearPossibleActions_TS() {
			lock (_unitLock) {
				possibleActions.Clear();
			}
		}

		public override string ToString () {
			return player.Colour + " " + type.ID + " (" + offset.x + ", " + offset.y + ")";;
		}

    }
}