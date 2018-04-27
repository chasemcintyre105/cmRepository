using System.Collections.Generic;

namespace RuleEngine {
	
	public class IfElseStatement : BlockContainingStatement {
        
		public IfElseStatement(Engine E) : base(E) {
			ArgumentList.SetArgument(0, NullValue.Instance.NewRef());
			
			Block newBlock = new Block(E);
			newBlock.AddStatement(VoidStatement.Instance); // Default initial void statement
			
			ArgumentList.SetArgument(1, newBlock.NewRef());
			
			newBlock = new Block(E);
			newBlock.AddStatement(VoidStatement.Instance); // Default initial void statement
			
			ArgumentList.SetArgument(2, newBlock.NewRef());
		}

		public IfElseStatement(Engine E, bool clean) : base(E) {
			ArgumentList.SetArgument(0, NullValue.Instance.NewRef());
			
			Block newBlock = new Block(E);
			if (!clean)
				newBlock.AddStatement(VoidStatement.Instance); // Default initial void statement

			ArgumentList.SetArgument(1, newBlock.NewRef());
			
			newBlock = new Block(E);
			if (!clean)
				newBlock.AddStatement(VoidStatement.Instance); // Default initial void statement

			ArgumentList.SetArgument(2, newBlock.NewRef());
		}
		
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Condition", typeof(BooleanValue), "The expression that determines which set of rules will apply");
            ArgumentList.DefineArgument("TrueBlock", typeof(Block), "This block is chosen if the expression is true");
            ArgumentList.DefineArgument("ElseBlock", typeof(Block), "This block is chosen if the expression is false");
		}
		
		public void SetExpression(Expression e) {
			ArgumentList.SetArgument(0, e);
		}

		public Block GetTrueBlock() {
			return (Block) ArgumentList.GetArgument(1).Instance();
		}
		
		public Block GetElseBlock() {
			return (Block) ArgumentList.GetArgument(2).Instance();
		}
		
		public override string GetDescription() {
			return "Allows only one set of rules to apply given a condition.";
		}

        public override string ToString() {
            return "If Else";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            bool condition = argsByName["Condition"].CalculateValue<BooleanValue>(RuleExecutor).value;
            if (condition) {
                argsByName["TrueBlock"].CalculateValue<VoidStatement>(RuleExecutor);
            } else {
                argsByName["ElseBlock"].CalculateValue<VoidStatement>(RuleExecutor);
            }
        }
    }

}