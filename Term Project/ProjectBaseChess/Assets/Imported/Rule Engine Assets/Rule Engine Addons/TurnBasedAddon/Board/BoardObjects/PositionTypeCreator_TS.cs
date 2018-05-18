using RuleEngine;

namespace RuleEngineAddons.TurnBased {
	
	public class PositionTypeCreator_TS {

		private PositionObjectRegistry PositionRegistry;
		
		private PositionType newPositionType;

		// Place a position using an absolute offset from the origin
		public PositionTypeCreator_TS(Engine E, string Name) {
            PositionRegistry = E.ObjectRegistries[typeof(Position)] as PositionObjectRegistry;

            newPositionType = new PositionType(Name);
		}

        public PositionTypeCreator_TS SetNotInteractable() {
            newPositionType.interactable = false;
            return this;
        }
        
        public void Finalise(out PositionType type) {
			type = newPositionType;
			Finalise();
		}

		public void Finalise() {

            // Register the position type
            PositionRegistry.RegisterObjectType_TS(newPositionType);

		}
        
	}
	
}
