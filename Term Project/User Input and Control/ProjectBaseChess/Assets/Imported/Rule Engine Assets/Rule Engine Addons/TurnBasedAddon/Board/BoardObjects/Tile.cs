using UnityEngine;
using System.Collections.Generic;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class Tile : IBoardObject {

		private readonly object _tileLock = new object();

        public readonly int ID;
        public readonly TileType type;
        public bool Placemarker; // Used to place tiles in-game

        private GameObject gameObject;
        private IBoardObjectAttachment Attachment;
		private Vector3 offset;
		private Dictionary<Vector3, Position> Positions;

		public Tile(int ID, TileType type) {
            this.ID = ID;
            this.type = type;

            _tileLock = new object();

			Positions = new Dictionary<Vector3, Position>();
            Placemarker = false;

		}

        public int GetID() {
            return ID;
        }

        public IObjectType GetObjectType() {
            return type;
        }

        public IEnumerable<Position> GetAllPositions_TS() {
            lock (_tileLock) {
                return Positions.Values;
            }
        }

		public Position GetPosition_TS(Vector3 offset) {
			lock(_tileLock) {
				Assert.True("Tile has position " + offset, Positions.ContainsKey(offset));
				return Positions[offset];
			}
		}
		
		public void SetPosition_TS(Vector3 offset, Position position) {
			lock(_tileLock) {
				Assert.False("Tile does not yet have position " + offset, Positions.ContainsKey(offset));
				Positions.Add(offset, position);
			}
		}
		
		public bool HasPosition_TS(Vector3 offset) {
			lock(_tileLock) {
				return Positions.ContainsKey(offset);
			}
		}

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

		public void SetOffset_TS(Vector3 position) { 
			lock (_tileLock) {
				this.offset = position;
			}
		}

		public Vector3 GetOffset_TS() { 
			lock (_tileLock) {
				return offset; 
			}
		}

		public override string ToString() {
			return type.ID + " (" + offset.x + ", " + offset.y + ")";
		}

	}

}