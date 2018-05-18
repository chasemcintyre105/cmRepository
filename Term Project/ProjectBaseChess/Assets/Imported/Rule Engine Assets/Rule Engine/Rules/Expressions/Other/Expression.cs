using System.Collections.Generic;

namespace RuleEngine {

    public abstract class Expression : Value {

        public abstract RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName);

        public Expression(Engine E) : base (E) { }

		public override bool IsEqualTo (Value value) {
			Assert.Never("Should not happen: Expression was compared via IsEqualTo");
			return false;
		}

        public override CompiledRuleExecutable NewCRE() {
            return new CREExpression(E, this, CalculateExpression);
        }

    }

}