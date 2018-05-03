using UnityEngine;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class TileCreator_TS {

		private Engine E;
		private PositionObjectRegistry PositionRegistry;
		private TileObjectRegistry TileRegistry;
		private float sqrTolerance;

		private Tile newTile;

		// Place a tile using an absolute offset from the origin
		public TileCreator_TS(Engine E, TileType type, Vector3 newTileOffset) {
			this.E = E;
            PositionRegistry = E.ObjectRegistries[typeof(Position)] as PositionObjectRegistry;
            TileRegistry = E.ObjectRegistries[typeof(Tile)] as TileObjectRegistry;
            sqrTolerance = E.GetManager<BoardManager>().sqrPositioningTolerance;

            MakeAndInitialiseTile(type, newTileOffset);

		}

		// Place a tile relative to another tile and join on a position of each one
		public TileCreator_TS(Engine E, TileType type, int newTileJoiningPositionOffsetIndex, Tile relativeTile, int relativeTileJoiningPositionOffsetIndex) {
			this.E = E;
            PositionRegistry = E.ObjectRegistries[typeof(Position)] as PositionObjectRegistry;
            TileRegistry = E.ObjectRegistries[typeof(Tile)] as TileObjectRegistry;
            sqrTolerance = E.GetManager<BoardManager>().sqrPositioningTolerance;

            // Find tile offset
            Vector3 relativeTileJoiningPositionOffset = relativeTile.type.Positions[relativeTileJoiningPositionOffsetIndex];
			Vector3 newTileJoiningPositionOffset = type.Positions[newTileJoiningPositionOffsetIndex];
			Position joiningPosition = relativeTile.GetPosition_TS(relativeTileJoiningPositionOffset);
			Vector3 newTileOffset = joiningPosition.GetOffset_TS() - newTileJoiningPositionOffset;

			MakeAndInitialiseTile(type, newTileOffset);
			
		}

		// Like above except a displacement vector is added between the two joining positions
		public TileCreator_TS(Engine E, TileType type, int newTileJoiningPositionOffsetIndex, Tile relativeTile, int relativeTileJoiningPositionOffsetIndex, Vector3 displacement) {
			this.E = E;
            PositionRegistry = E.ObjectRegistries[typeof(Position)] as PositionObjectRegistry;
            TileRegistry = E.ObjectRegistries[typeof(Tile)] as TileObjectRegistry;
            sqrTolerance = E.GetManager<BoardManager>().sqrPositioningTolerance;

            // Find tile offset
            Vector3 relativeTileJoiningPositionOffset = relativeTile.type.Positions[relativeTileJoiningPositionOffsetIndex];
			Vector3 newTileJoiningPositionOffset = type.Positions[newTileJoiningPositionOffsetIndex];
			Position joiningPosition = relativeTile.GetPosition_TS(relativeTileJoiningPositionOffset);
            
            Assert.True("The tile type has the displacement requested", relativeTile.type.PositionTypes[relativeTileJoiningPositionOffset].Displacements.Contains(displacement));

			Vector3 newTileOffset = joiningPosition.GetOffset_TS() - newTileJoiningPositionOffset + displacement;

			MakeAndInitialiseTile(type, newTileOffset);

            // Register the displacement with the joining position so that this new edge can be rediscovered later
            //joiningPosition.AddDisplacment_TS(displacement); // Not yet
            
		}

        private void MakeAndInitialiseTile(TileType type, Vector3 offset) {

            // Create the new tile and give it the calculated offset
            newTile = new Tile(TileRegistry.GenerateObjectUID_TS(), type);
            newTile.SetOffset_TS(offset);

        }

        public TileCreator_TS AddAdjacentPosition(int index, Position adjacentPosition) {

			Vector3 offset = newTile.type.Positions[index];
			newTile.SetPosition_TS(offset, adjacentPosition);

			return this;
		}
		
        public TileCreator_TS SetAsPlacemarker() {
            newTile.Placemarker = true;
            return this;
        }

        public TileCreator_TS Register() {
            TileRegistry.RegisterObject_TS(newTile);
            return this;
        }

        public void Finalise(out Tile tile) {
			tile = newTile;
			Finalise();
		}

        public void Finalise(out IBoardObject tile) {
            tile = newTile;
            Finalise();
        }

        public void Finalise() {
            
            // Configure the new tile
            E.EffectFactory.EnqueueNewEffect<IConfigureNewBoardObjectEffect>(E, newTile);

            // Add the tile to the game
            if (!newTile.Placemarker) {
                FillPositions();
                E.GetManager<BoardManager>().ApplyGameModification_TS(new AddTileGameModification(E, newTile));
            } 

        }

		private void FillPositions() {
			foreach (Vector3 positionOffset in newTile.type.PositionOffsets) {
				Position position = PositionRegistry.GetClosestAtOffsetWithinTolerance_TS(newTile.GetOffset_TS() + positionOffset, sqrTolerance);

				if (position == null) { // Create new position

					// If there is no position already in this place, create a new position and add it to the tile
					new PositionCreator_TS(E, newTile.type.PositionTypes[positionOffset].positionType, newTile, positionOffset).Finalise(out position);
					newTile.SetPosition_TS(positionOffset, position);

				} else { // Fill with existing position

					// Add the already existant position to this new tile
					if (!newTile.HasPosition_TS(positionOffset))
						newTile.SetPosition_TS(positionOffset, position);

				}

                if (!position.GetAllAdjacentTiles_TS().ContainsValue(newTile))
                    position.AddAdjacentTile_TS(newTile);
			}
		}

	}
	
}
