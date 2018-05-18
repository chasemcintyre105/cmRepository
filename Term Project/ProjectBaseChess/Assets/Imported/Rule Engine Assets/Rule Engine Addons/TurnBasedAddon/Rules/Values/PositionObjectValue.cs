using System;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class PositionObjectValue : ObjectValue {
		
		public override string GetSelectionPanelCategory() {
			return "Positions";
		}
        
		public PositionObjectValue(Engine E, PositionObjectTypeValue potv, Position Instance) : base(E, potv, Instance) {

		}

		public Position GetInstance() {
			return (Position) Instance;
		}

		public override string GetDescription() {
			return "Represents a position in the game. In this case " + Instance.ToString();
		}

        public override string ToString() {
            return "PositionObjectValue: " + ((Position) Instance).ToString();
        }

    }

}