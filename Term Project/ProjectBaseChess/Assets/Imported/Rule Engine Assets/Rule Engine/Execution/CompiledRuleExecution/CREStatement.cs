using System;
using System.Collections.Generic;

namespace RuleEngine {

    public class CREStatement : CompiledRuleExecutable {

        protected Action<RuleExecutor, List<CREArgument>, Dictionary<string, CREArgument>> calc;

        public CREStatement(Engine E, RuleComponent associatedObj, Action<RuleExecutor, List<CREArgument>, Dictionary<string, CREArgument>> calc) : base(E, associatedObj) {
            this.calc = calc;
        }

        public override O CalculateValue<O>(RuleExecutor RuleExecutor) {
            calc.Invoke(RuleExecutor, argsByOrder, argsByName);
            Assert.True("Statements always return void", VoidStatement.Instance is O);
            return VoidStatement.Instance as O;
        }

        public override bool IsValueType(Type type) {
            return typeof(VoidStatement) == type;
        }

    }

}
