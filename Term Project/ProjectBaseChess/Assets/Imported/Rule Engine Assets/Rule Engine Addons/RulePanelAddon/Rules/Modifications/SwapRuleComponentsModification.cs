using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class SwapRuleComponentsModification : RuleModification {

		private RuleComponent FirstObject;
		private RuleComponent SecondObject;
		private ArgumentAccessor FirstObjectAccessor;
		private ArgumentAccessor SecondObjectAccessor;

		public SwapRuleComponentsModification(Rule AssociatedRule, RuleComponent FirstObject, ArgumentAccessor FirstObjectAccessor, RuleComponent SecondObject, ArgumentAccessor SecondObjectAccessor) {
            this.AssociatedRule = AssociatedRule;
            this.FirstObject = FirstObject;
			this.SecondObject = SecondObject;
			this.FirstObjectAccessor = FirstObjectAccessor;
			this.SecondObjectAccessor = SecondObjectAccessor;
		}

		protected override void Apply() {
			Assert.Same("First object is in first object accessor", FirstObject, FirstObjectAccessor.Argument.Instance());
			Assert.Same("First object is in first object accessor", SecondObject, SecondObjectAccessor.Argument.Instance());
			FirstObjectAccessor.Argument = SecondObject.NewRef();
			SecondObjectAccessor.Argument = FirstObject.NewRef();
		}

		protected override void Undo() {
			Assert.Same("Second object is in the position of the first", SecondObject, SecondObjectAccessor.Argument.Instance());
			Assert.Same("First object is in the position of the second", FirstObject, FirstObjectAccessor.Argument.Instance());

			FirstObjectAccessor.Argument = FirstObject.NewRef();
			SecondObjectAccessor.Argument = SecondObject.NewRef();
		}
		
		public override string ToString () {
			return "SwapRuleComponentsModification";
		}

	}

}
