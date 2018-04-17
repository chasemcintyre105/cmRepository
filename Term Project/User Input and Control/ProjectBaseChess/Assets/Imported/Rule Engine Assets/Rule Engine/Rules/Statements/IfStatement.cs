using System.Collections.Generic;

namespace RuleEngine {
	
	public class IfStatement : BlockContainingStatement {
        
		public IfStatement(Engine E) : base(E) {
			ArgumentList.SetArgument(0, NullValue.Instance.NewRef());
			
			Block newBlock = new Block(E);
			newBlock.AddStatement(VoidStatement.Instance); // Default initial void statement
			
			ArgumentList.SetArgument(1, newBlock.NewRef());
		}

		public IfStatement(Engine E, bool clean) : base(E) {
			ArgumentList.SetArgument(0, NullValue.Instance.NewRef());

			Block newBlock = new Block(E);
			if (!clean)
				newBlock.AddStatement(VoidStatement.Instance); // Default initial void statement

			ArgumentList.SetArgument(1, newBlock.NewRef());
		}
        
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Condition", typeof(BooleanValue), "The expression that determines whether the following set of rules will apply");
            ArgumentList.DefineArgument("TrueBlock", typeof(Block), "This block is chosen if the expression is true");
		}

		public void SetExpression(Expression e) {
			ArgumentList.SetArgument(0, e);
		}

		public Block GetTrueBlock() {
			return (Block) ArgumentList.GetArgument(1).Instance();
		}
		
		public override string GetDescription() {
			return "Allows a set of rules to apply only if a given condition is true.";
		}

        public override string ToString() {
            return "If";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            bool condition = argsByName["Condition"].CalculateValue<BooleanValue>(RuleExecutor).value;
            if (condition) {
                argsByName["TrueBlock"].CalculateValue<VoidStatement>(RuleExecutor);
            }
        }
    }

}