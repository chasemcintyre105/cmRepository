using UnityEngine;
using System.Collections.Generic;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class TileType : IBoardObjectType {

        // TS methods are not necessary for this since we assume that these values do not changes once set
        public string ID;
        public float MinimumHeight = 1f;
        public float MaximumHeight = 1f;
        public bool AllowRotation = false;
        public float Rarity = 1;

        public IEnumerable<Vector3> PositionOffsets {
            get {
                return PositionTypes.Keys;
            }
        }

        public bool HasPosition(Vector3 offset) {
            return PositionTypes.ContainsKey(offset);
        }

        public List<Vector3> Positions;
        public Dictionary<Vector3, PositionProfile> PositionTypes;

        public class PositionProfile {
            public PositionType positionType;
            public List<Vector3> Neighbours = new List<Vector3>();
            public List<Vector3> Displacements = new List<Vector3>();
        }
        
        private List<GameObject> _Templates;
		public GameObject Template {
			get {
				return _Templates[Random.Range(0, _Templates.Count)];
			}
		}

		public TileType(Engine E, string ID, List<GameObject> Templates) {
            this.ID = ID;
			this._Templates = Templates;

            Positions = new List<Vector3>();
            PositionTypes = new Dictionary<Vector3, PositionProfile>();

		}

		public TileType(Engine E, string ID, List<GameObject> Templates, Vector3[] initialOffsets, PositionType[] initialTypes) {
			Assert.True("There are tile templates", Templates.Count > 0);
			this.ID = ID;
			this._Templates = Templates;

            Assert.Same("The offsets and types arrays are the same length", initialOffsets.Length, initialTypes.Length);

            Positions = new List<Vector3>();
            PositionTypes = new Dictionary<Vector3, PositionProfile>();

            for (int i = 0; i < initialOffsets.Length; i++) {
                AddPositionOffset(initialOffsets[i], initialTypes[i]);
            }

        }

        public string GetID() {
            return ID;
        }

        public void AddPositionOffset(Vector3 offset, PositionType type) {
            Positions.Add(offset);
            PositionTypes.Add(offset, new PositionProfile() {
                positionType = type
            });
        }

        public void AddTemplate(GameObject template) {
            _Templates.Add(template);
        }

	}

}
