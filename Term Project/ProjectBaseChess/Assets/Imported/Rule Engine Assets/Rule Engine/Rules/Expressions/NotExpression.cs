using System;
using System.Collections.Generic;

namespace RuleEngine {
	
	public class NotExpression : Expression {

		public override Type GetReturnType() {
			return typeof(BooleanValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Logic";
		}

		public NotExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Value", typeof(BooleanValue), "If this is true, the Not statement will give false, and vise versa.");
		}
		
		public void SetValue(Value val) {
			ArgumentList.SetArgument(0, val);
		}
		
		public override string GetDescription() {
			return "Takes a value of either true or false and gives the other one.";
		}

        public override string ToString() {
            return "Not";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            bool booleanValue = argsByOrder[0].CalculateValue<BooleanValue>(RuleExecutor).value;
            return new BooleanValue(E, !booleanValue);
        }
    }

}