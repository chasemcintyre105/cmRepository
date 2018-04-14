using System;
using System.Collections.Generic;

namespace RuleEngine {

    public abstract class Statement : RuleComponent {

        public abstract void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName);

        public Statement(Engine E) : base(E) {

        }

        public override Type GetReturnType() {
            return typeof(VoidStatement);
        }

        public override string GetSelectionPanelCategory() {
            return "Statements";
        }

        public override CompiledRuleExecutable NewCRE() {
            return new CREStatement(E, this, ExecuteStatement);
        }

    }

}