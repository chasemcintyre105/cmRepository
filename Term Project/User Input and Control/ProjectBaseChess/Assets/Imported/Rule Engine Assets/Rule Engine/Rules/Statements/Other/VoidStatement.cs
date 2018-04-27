using System.Collections.Generic;

namespace RuleEngine {

    public class VoidStatement : Statement {

		public static VoidStatement Instance = new VoidStatement();

        public override void DefineArguments() {
		}

		public VoidStatement() : base(RuleEngineController.E) {
            Editability = RuleComponentEditability.Editable;
		}
		
		public override string GetDescription() {
			return "A statement that represents the absence of a statement";
		}

        public override string ToString() {
            return "Void Statement";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            Assert.Never("Void should never be executed");
        }
    }

}