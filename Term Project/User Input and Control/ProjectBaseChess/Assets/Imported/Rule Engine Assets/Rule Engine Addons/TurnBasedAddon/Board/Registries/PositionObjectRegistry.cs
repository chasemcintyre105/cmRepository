using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class PositionObjectRegistry : BoardObjectRegistry<Position, PositionType, PositionObjectValue, PositionObjectTypeValue> {

        private static ObjectValueCreator CreatePositionValue = delegate (Engine E, Position o, PositionObjectTypeValue otv) {
            return new PositionObjectValue(E, otv, o);
        };

        private static ObjectTypeValueCreator CreatePositionTypeValue = delegate (Engine E, PositionType ot) {
            return new PositionObjectTypeValue(E, ot);
        };

        public PositionObjectRegistry(Engine E) : base(E, CreatePositionValue, CreatePositionTypeValue, true) {
        }
        
	}

}
