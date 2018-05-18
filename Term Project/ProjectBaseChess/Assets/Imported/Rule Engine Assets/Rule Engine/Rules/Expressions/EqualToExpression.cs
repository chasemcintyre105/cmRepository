using System;
using System.Collections.Generic;

namespace RuleEngine {
	
	public class EqualToExpression : Expression {

		public override Type GetReturnType() {
			return typeof(BooleanValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Comparisons";
		}

		public EqualToExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Left", typeof(Value), "If this is the same as the right hand side, this statement will give true. Otherwise false.", false);
            ArgumentList.DefineArgument("Right", typeof(Value), "If this is the same as the left hand side, this statement will give true. Otherwise false.", false);
        }
        
		public void SetValues(Value left, Value right) {
			ArgumentList.SetArgument(0, left);
			ArgumentList.SetArgument(1, right);
		}
		
		public override string GetDescription() {
			return "Determines if two things are equal.";
		}

        public override string ToString() {
            return "Equal To";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            Value left = argsByOrder[0].CalculateValue<Value>(RuleExecutor);
            Value right = argsByOrder[1].CalculateValue<Value>(RuleExecutor);
            return new BooleanValue(E, left.IsEqualTo(right));
        }
    }

}