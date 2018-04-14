using System;
using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class TypeOfUnitExpression : Expression {

		public override Type GetReturnType() {
			return typeof(UnitObjectTypeValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Attributes";
		}

		public TypeOfUnitExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Unit", typeof(UnitObjectValue), "The unit of which to determine the type.");
		}
        
		public override string GetDescription() {
			return "Gives the type of a unit.";
		}

        public override string ToString() {
            return "TypeOfUnit";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            UnitObjectValue argUnit = argsByOrder[0].CalculateValue<UnitObjectValue>(RuleExecutor);
            return argUnit.TypeValue;
        }
    }

}