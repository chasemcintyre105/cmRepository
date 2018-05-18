using UnityEngine;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class UnitCreator_TS {

		private Engine E;
		private Unit newUnit;
        private UnitObjectRegistry UnitRegistry;

        public UnitCreator_TS(Engine E, Vector3 offset, Player player, UnitType type) {
			this.E = E;
            UnitRegistry = E.ObjectRegistries[typeof(Unit)] as UnitObjectRegistry;

            Assert.True("Unit's position is on the game board", E.GetManager<BoardManager>().IsPositionWithinTolerance_TS(offset));

			MakeAndInitialiseUnit(type, offset, player);

		}

		public UnitCreator_TS(Engine E, Vector3 offset, Player player, string typeID) {
			this.E = E;
            UnitRegistry = E.ObjectRegistries[typeof(Unit)] as UnitObjectRegistry;

            Assert.True("Unit's position is on the game board", E.GetManager<BoardManager>().IsPositionWithinTolerance_TS(offset));

			MakeAndInitialiseUnit(UnitRegistry.GetObjectTypeByID_TS(typeID), offset, player);

		}

		public UnitCreator_TS(Engine E, Position position, Player player, UnitType type) {
			this.E = E;
            UnitRegistry = E.ObjectRegistries[typeof(Unit)] as UnitObjectRegistry;

            MakeAndInitialiseUnit(type, position.GetOffset_TS(), player);

		}

		public UnitCreator_TS(Engine E, Position position, Player player, string typeID) {
			this.E = E;
            UnitRegistry = E.ObjectRegistries[typeof(Unit)] as UnitObjectRegistry;

            MakeAndInitialiseUnit(UnitRegistry.GetObjectTypeByID_TS(typeID), position.GetOffset_TS(), player);

		}

        private void MakeAndInitialiseUnit(UnitType type, Vector3 offset, Player player) {

            // Create the new unit
            newUnit = new Unit(UnitRegistry.GenerateObjectUID_TS(), type);
            newUnit.SetOffset_TS(offset);
            newUnit.player = player;

        }

        public UnitCreator_TS SetAsPlacemarker() {
            newUnit.Placemarker = true;
            return this;
        }

        public UnitCreator_TS Register() {
            UnitRegistry.RegisterObject_TS(newUnit);
            return this;
        }

        public void Finalise(out Unit unit) {
			unit = newUnit;
			Finalise();
		}

		public void Finalise() {
            
            // Configure the new unit
            E.EffectFactory.EnqueueNewEffect<IConfigureNewBoardObjectEffect>(E, newUnit);

            // Add the unit to the game
            if (!newUnit.Placemarker)
                E.GetManager<BoardManager>().ApplyGameModification_TS(new AddUnitGameModification(E, newUnit));

        }

	}
	
}
