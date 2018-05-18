using System;
using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {
	
	public class DirectionAdditionExpression : Expression {

		public override Type GetReturnType() {
			return typeof(DirectionValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Directions";
		}

		public DirectionAdditionExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Left", typeof(DirectionValue), "The first direction to be added");
            ArgumentList.DefineArgument("Right", typeof(DirectionValue), "The second direction to be added");
        }
        
		public override string GetDescription() {
			return "An expression that adds two directions together";
		}

        public override string ToString() {
            return "+";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            DirectionValue argDir1 = argsByOrder[0].CalculateValue<DirectionValue>(RuleExecutor);
            DirectionValue argDir2 = argsByOrder[1].CalculateValue<DirectionValue>(RuleExecutor);

            return new DirectionValue(E, argDir1.value + argDir2.value);
        }
    }

}