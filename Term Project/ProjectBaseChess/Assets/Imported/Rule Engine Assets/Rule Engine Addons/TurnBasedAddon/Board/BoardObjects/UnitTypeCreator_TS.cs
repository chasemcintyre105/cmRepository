using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class UnitTypeCreator_TS {

        private UnitObjectRegistry UnitRegistry;
		
		private UnitType newUnitType;

		// Place a Unit using an absolute offset from the origin
		public UnitTypeCreator_TS(Engine E, string Name) {
            UnitRegistry = E.ObjectRegistries[typeof(Unit)] as UnitObjectRegistry;

            newUnitType = new UnitType(E, Name);
		}

        public UnitTypeCreator_TS SetToGroupingType() {
            newUnitType.IsGroupingType = true;
            return this;
        }
        
        public void Finalise(out UnitType type) {
			type = newUnitType;
			Finalise();
		}

		public void Finalise() {

            // Register the unit type
            UnitRegistry.RegisterObjectType_TS(newUnitType);
            
        }

    }
	
}
