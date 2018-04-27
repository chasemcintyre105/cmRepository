using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class RemoveRuleComponentModification : RuleModification {

		public RuleComponent associatedObject { get; private set; }
		public ArgumentAccessor associatedObjectAccessor { get; private set; }

		private bool isStatement;
		
		public RemoveRuleComponentModification(Rule AssociatedRule, RuleComponent associatedObject, ArgumentAccessor associatedObjectAccessor) {
            this.AssociatedRule = AssociatedRule;
			this.associatedObject = associatedObject;
			this.associatedObjectAccessor = associatedObjectAccessor;
		}
		
		protected override void Apply() {
			Assert.True("The object is in its original place", associatedObjectAccessor.Argument.Instance() == associatedObject);

			if (associatedObjectAccessor.ContainsAcceptableType(typeof(VoidStatement))) {

				isStatement = true;

				// Get a reference to the parent block
				Block block = (Block) associatedObjectAccessor.obj;
				Assert.NotNull("The associate object accessor's object is a block statement", block);

				// Remove the statement from the block
				block.RemoveStatement(associatedObjectAccessor.index);

			} else {

				isStatement = false;

				// Replace the value with the null value
				associatedObjectAccessor.Argument = NullValue.Instance.NewRef();

			}

		}
		
		protected override void Undo() {

			// Check that the accessor accesses either the void statement or the null value
			if (isStatement) {

				// Get a reference to the parent block
				Block block = (Block) associatedObjectAccessor.obj;
				Assert.NotNull("The associate object accessor's object is a block statement", block);

				// Add the original statement back in at the appropriate place in the block
				block.AddStatementAt(associatedObject.NewRef(), associatedObjectAccessor.index);

			} else {

				Assert.True("The accessor accesses the null value", associatedObjectAccessor.Argument.Instance() == NullValue.Instance);

				// Add the original value back in
				associatedObjectAccessor.Argument = associatedObject.NewRef();

			}

			Assert.True("The object is in its original place", associatedObjectAccessor.Argument.Instance() == associatedObject);

		}
		
		public override string ToString () {
			return "RemoveStatementModification";
		}

	}

}