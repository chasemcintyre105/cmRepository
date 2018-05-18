using System;
using System.Collections.Generic;

namespace RuleEngine {

    public class CREExpression : CompiledRuleExecutable {

        private Type returnType;
        private Func<RuleExecutor, List<CREArgument>, Dictionary<string, CREArgument>, RuleComponent> calc;

        public CREExpression(Engine E, RuleComponent associatedObj, Func<RuleExecutor, List<CREArgument>, Dictionary<string, CREArgument>, RuleComponent> calc) : base(E, associatedObj) {
            returnType = associatedObj.GetReturnType();
            this.calc = calc;
        }

        public override O CalculateValue<O>(RuleExecutor RuleExecutor) {
            RuleComponent obj = calc.Invoke(RuleExecutor, argsByOrder, argsByName);

            Assert.True("The requested type is the type of the value.", obj is O);
            return obj as O;
        }

        public override bool IsValueType(Type type) {
            return type == returnType;
        }

    }

}
