using RuleEngine;
using System;

namespace RuleEngineAddons.TurnBased {

	public class UnitObjectValue : ObjectValue {
        
		public override string GetSelectionPanelCategory() {
			return "Units";
		}
        
        public UnitObjectValue(Engine E, UnitObjectTypeValue ObjTypeValue, Unit Instance) : base(E, ObjTypeValue, Instance) {

        }

        public Unit GetInstance() {
			return (Unit) Instance;
		}

		public override string GetDescription() {
			return "Represents a unit in the game. In this case " + ToString();
		}

        public override string ToString() {
            return "UnitObjectValue: " + ((Unit) Instance).ToString();
        }

    }

}