using System.Collections.Generic;
using System.Threading;

namespace RuleEngine {

	public class Block : Statement {
        
		public Block(Engine E) : base(E) {
		}

		public Block(Engine E, Statement initialStatement) : base(E) {
			AddStatement(initialStatement.NewRef());
		}

		public void AddStatement(RuleComponent obj) {
			AddStatement(obj.NewRef());
		}

		public void AddStatement(RuleComponentReference obj) {
			Argument newArg = new Argument();

			newArg.parentObject = this;
			newArg.index = ArgumentList.argsByOrder.Count;
			newArg.reference = obj;
			newArg.AcceptableArgumentTypes.Add(typeof(VoidStatement));
            newArg.name = ArgumentList.argsByOrder.Count.ToString();

            ArgumentList.argsByOrder.Add(newArg);
		}

		public void AddStatementAt(RuleComponentReference obj, int index) {
			Argument newArg = new Argument();
			
			newArg.parentObject = this;
			newArg.index = ArgumentList.argsByOrder.Count;
			newArg.reference = obj;
			newArg.AcceptableArgumentTypes.Add(typeof(VoidStatement));
			
			ArgumentList.argsByOrder.Insert(index, newArg);
            ArgumentList.UpdateArgumentOfBlockFrom(index);
        }

		public void RemoveStatement(int index) {
			ArgumentList.argsByOrder.RemoveAt(index);
			ArgumentList.UpdateArgumentOfBlockFrom(index);
		}
        
		public override void DefineArguments() {
            // Starts empty
		}
		
		public override string GetDescription() {
			return "";
		}

        public override string ToString() {
            return "<Block Statement>";
        }
        
        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {

            // For each statement in the block, in order
            for (int i = 0; i < argsByOrder.Count; i++) {

                // Execute the statement
                argsByOrder[i].CalculateValue<VoidStatement>(RuleExecutor);

                // Play nicely with other threads
                Thread.Sleep(0); 

            }

        }

    }

}
