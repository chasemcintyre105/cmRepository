using RuleEngine;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public class PositionCreator_TS {

		private Engine E;
		private PositionObjectRegistry PositionRegistry;
		private UnitObjectRegistry UnitRegistry;

		private Position newPosition;

		// Create a position relative to a tile
		public PositionCreator_TS(Engine E,
		                          PositionType type,
		                          Tile parentTile,
		                          Vector3 relativePositionOffsetFromTile) {
			this.E = E;
            PositionRegistry = E.ObjectRegistries[typeof(Position)] as PositionObjectRegistry;
            UnitRegistry = E.ObjectRegistries[typeof(Unit)] as UnitObjectRegistry;

            Assert.True(parentTile.type.ID + " tiles allow a position with offset " + relativePositionOffsetFromTile, parentTile.type.HasPosition(relativePositionOffsetFromTile));
			Assert.True("This tile does not yet contain a position with offset " + relativePositionOffsetFromTile, !parentTile.HasPosition_TS(relativePositionOffsetFromTile));

			MakeAndInitialisePosition(type, relativePositionOffsetFromTile, parentTile);

		}

        // Create a lone position
        public PositionCreator_TS(Engine E,
                                  PositionType type,
                                  Vector3 absoluteOffset) {
            this.E = E;
            PositionRegistry = E.ObjectRegistries[typeof(Position)] as PositionObjectRegistry;
            UnitRegistry = E.ObjectRegistries[typeof(Unit)] as UnitObjectRegistry;

            MakeAndInitialisePosition(type, absoluteOffset, null);

        }

        private void MakeAndInitialisePosition(PositionType type, Vector3 offset, Tile parentTile) {

            // Create the new position
            if (parentTile == null) {
                newPosition = new Position(PositionRegistry.GenerateObjectUID_TS(), UnitRegistry, type, offset);
            } else {
                newPosition = new Position(PositionRegistry.GenerateObjectUID_TS(), UnitRegistry, type, offset, parentTile);
            }
        }

        public PositionCreator_TS AddAdjacentTile(Tile adjacentTile) {
            newPosition.AddAdjacentTile_TS(adjacentTile);
            return this;
		}

        public PositionCreator_TS SetAsPlacemarker(IBoardObject placingObject) {
            newPosition.Placemarker = true;
            newPosition.placingObject = placingObject;
            return this;
        }

        public PositionCreator_TS Register() {
            PositionRegistry.RegisterObject_TS(newPosition);
            return this;
        }

        public void Finalise(out Position position) {
			position = newPosition;
			Finalise();
		}

		public void Finalise() {
            
            // Configure the new position
            E.EffectFactory.EnqueueNewEffect<IConfigureNewBoardObjectEffect>(E, newPosition);

            // Add the position to the game
            if (!newPosition.Placemarker)
                E.GetManager<BoardManager>().ApplyGameModification_TS(new AddPositionGameModification(E, newPosition));

        }

	}
	
}
