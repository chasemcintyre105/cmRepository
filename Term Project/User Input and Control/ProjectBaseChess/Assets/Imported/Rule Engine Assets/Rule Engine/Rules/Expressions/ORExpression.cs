using System;
using System.Collections.Generic;

namespace RuleEngine {
	
	public class OrExpression : Expression {

		public override Type GetReturnType() {
			return typeof(BooleanValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Logic";
		}

		public OrExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Left", typeof(BooleanValue), "If this or the right hand side are true, this Or statement will give true. Otherwise false.");
            ArgumentList.DefineArgument("Right", typeof(BooleanValue), "If this or the left hand side are true, this Or statement will give true. Otherwise false.");
		}
        
		public void SetValues(Value left, Value right) {
			ArgumentList.SetArgument(0, left);
			ArgumentList.SetArgument(1, right);
		}
		
		public override string GetDescription() {
			return "Takes two values of either true or false and gives back true if at least one of them is true.";
		}

        public override string ToString() {
            return "Or";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            bool left = argsByOrder[0].CalculateValue<BooleanValue>(RuleExecutor).value;
            bool right = argsByOrder[1].CalculateValue<BooleanValue>(RuleExecutor).value;
            return new BooleanValue(E, left || right);
        }
    }

}