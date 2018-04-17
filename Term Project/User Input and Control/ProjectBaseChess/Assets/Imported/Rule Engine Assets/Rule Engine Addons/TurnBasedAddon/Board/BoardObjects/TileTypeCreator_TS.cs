using System.Collections.Generic;
using UnityEngine;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class TileTypeCreator_TS {

        private TileObjectRegistry TileRegistry;
		
		private TileType newTileType;
        
		// Place a tile using an absolute offset from the origin
		public TileTypeCreator_TS(Engine E, string ID) {
            TileRegistry = E.ObjectRegistries[typeof(Tile)] as TileObjectRegistry;

            newTileType = new TileType(E, ID, new List<GameObject>());
		}

        public TileTypeCreator_TS AddTemplate(GameObject template) {
            newTileType.AddTemplate(template);
            return this;
        }

        public TileTypeCreator_TS SetHeightRange(float Minimum, float Maximum) {
            newTileType.MinimumHeight = Minimum;
            newTileType.MaximumHeight = Maximum;
			return this;
		}

        public TileTypeCreator_TS SetAllowRotation(bool Allow) {
            newTileType.AllowRotation = Allow;
            return this;
        }

        public TileTypeCreator_TS SetRarity(float rarity) {
            newTileType.Rarity = rarity;
            return this;
        }

        public TileTypeCreator_TS AddPositionType(Vector3 offset, PositionType positionType) {
            newTileType.AddPositionOffset(offset, positionType);
            return this;
        }

        public TileTypeCreator_TS AddPositionNeighbour(Vector3 positionOffset, Vector3 neighbourOffset) {
            Assert.True("TileType already has a position at the offset", newTileType.PositionTypes.ContainsKey(positionOffset));
            newTileType.PositionTypes[positionOffset].Neighbours.Add(neighbourOffset);
            return this;
        }

        public TileTypeCreator_TS AddPositionNeighbours(Vector3 positionOffset, List<Vector3> neighbourOffsets) {
            Assert.True("TileType already has a position at the offset", newTileType.PositionTypes.ContainsKey(positionOffset));
            newTileType.PositionTypes[positionOffset].Neighbours.AddRange(neighbourOffsets);
            return this;
        }

        public TileTypeCreator_TS SetAllPositionNeighbours(List<Vector3> neighbourOffsets) {
            foreach (TileType.PositionProfile profile in newTileType.PositionTypes.Values) {
                profile.Neighbours.AddRange(neighbourOffsets);
            }
            return this;
        }

        public TileTypeCreator_TS AddPositionDisplacement(Vector3 positionOffset, Vector3 displacement) {
            Assert.True("TileType already has a position at the offset", newTileType.PositionTypes.ContainsKey(positionOffset));
            newTileType.PositionTypes[positionOffset].Displacements.Add(displacement);
            return this;
        }

        public TileTypeCreator_TS AddPositionDisplacements(Vector3 positionOffset, List<Vector3> displacements) {
            Assert.True("TileType already has a position at the offset", newTileType.PositionTypes.ContainsKey(positionOffset));
            newTileType.PositionTypes[positionOffset].Displacements.AddRange(displacements);
            return this;
        }

        public TileTypeCreator_TS SetAllPositionDisplacements(List<Vector3> displacements) {
            foreach (TileType.PositionProfile profile in newTileType.PositionTypes.Values) {
                profile.Displacements.AddRange(displacements);
            }
            return this;
        }

        public void Finalise(out TileType type) {
			type = newTileType;
			Finalise();
		}

		public void Finalise() {

            // Register the tile type
            TileRegistry.RegisterObjectType_TS(newTileType);

        }

    }
	
}
