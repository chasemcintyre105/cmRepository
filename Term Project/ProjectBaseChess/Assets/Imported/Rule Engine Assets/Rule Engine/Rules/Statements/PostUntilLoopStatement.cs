using System.Collections.Generic;
using System.Threading;

namespace RuleEngine {
	
	public class PostUntilLoopStatement : BlockContainingStatement {
        
		public PostUntilLoopStatement(Engine E) : base(E) {
			ArgumentList.SetArgument(0, NullValue.Instance.NewRef());
			ArgumentList.SetArgument(1, E.IntegerObjectRegistry.CreateIntegerNumberValueFromInt_TS(10));

            Block newBlock = new Block(E);
			newBlock.AddStatement(VoidStatement.Instance); // Default initial void statement
			
			ArgumentList.SetArgument(2, newBlock.NewRef());
		}

		public PostUntilLoopStatement(Engine E, bool clean) : base(E) {
			ArgumentList.SetArgument(0, NullValue.Instance.NewRef());
			ArgumentList.SetArgument(1, E.IntegerObjectRegistry.CreateIntegerNumberValueFromInt_TS(10));

            Block newBlock = new Block(E);
			if (!clean)
				newBlock.AddStatement(VoidStatement.Instance); // Default initial void statement

			ArgumentList.SetArgument(2, newBlock.NewRef());
		}
		
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Condition", typeof(BooleanValue), "The expression that determines whether set of rules will apply again");
            ArgumentList.DefineArgument("MaxIterations", typeof(NumberValue), "The maximum number of iterations for the loop");
            ArgumentList.DefineArgument("LoopBlock", typeof(Block), "The block of statements that is evaluated each time the loop is run");
		}

		public void SetExpression(Expression e) {
			ArgumentList.SetArgument(0, e);
		}

		public Block GetLoopBlock() {
			return (Block) ArgumentList.GetArgument(2).Instance();
		}
		
		public override string GetDescription() {
			return "Allows a set of rules to apply more than once but only up until a given condition is becomes";
		}

        public override string ToString() {
            return "Until";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            NumberValue numberValue = argsByName["MaxIterations"].CalculateValue<NumberValue>(RuleExecutor);
            int MaxIterations = (int) numberValue.numberValue.GetValue();

            CREArgument block = argsByName["LoopBlock"];
            CREArgument condition = argsByName["Condition"];

            for (int i = 0; i < MaxIterations; i++) {

                // Execute the block
                block.CalculateValue<VoidStatement>(RuleExecutor);

                // Recalculate the condition
                BooleanValue argBool = condition.CalculateValue<BooleanValue>(RuleExecutor);
                
                if (argBool.value)
                    break;

                // Play nicely with other threads
                Thread.Sleep(0); 

            }
        }
    }

}