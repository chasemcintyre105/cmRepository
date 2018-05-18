using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class AddStatementModification : RuleModification {

		private Block block;
		public Statement associatedStatement { get; private set; }

		public AddStatementModification(Rule AssociatedRule, Block block) {
            this.AssociatedRule = AssociatedRule;
			this.block = block;
        }

		protected override void Apply() {
			associatedStatement = new VoidStatement();
            associatedStatement.Editability = block.Editability;
            block.AddStatement(associatedStatement.NewRef());
		}

		protected override void Undo() {
			Assert.True("Block is not empty", block.ArgumentList.argsByOrder.Count > 0);

			int index = block.ArgumentList.argsByOrder.Count - 1;
			Assert.True("Last statement in block is the statement placemarker", block.ArgumentList.argsByOrder[index].reference.Instance() == associatedStatement);

			block.ArgumentList.argsByOrder.RemoveAt(index);

		}
		
		public override string ToString () {
			return "AddStatementModification";
		}

	}

}

