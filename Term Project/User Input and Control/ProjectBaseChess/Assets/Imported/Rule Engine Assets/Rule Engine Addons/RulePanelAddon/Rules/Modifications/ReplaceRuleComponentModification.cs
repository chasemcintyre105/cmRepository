using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class ReplaceRuleComponentModification : RuleModification {

		public RuleComponent OldObject;
		public ArgumentAccessor OldObjectAccessor;
		public RuleComponent NewObject;
        public RuleComponent.RuleComponentEditability editibility;

		public ReplaceRuleComponentModification(Rule AssociatedRule, RuleComponent OldObject, ArgumentAccessor OldObjectAccessor, RuleComponent NewObject) {
            this.AssociatedRule = AssociatedRule;
			this.OldObject = OldObject;
			this.OldObjectAccessor = OldObjectAccessor;
			this.NewObject = NewObject;
		}

		protected override void Apply() {
			Assert.Same("OldObjectAccessor accesses the old object", OldObjectAccessor.Argument.Instance(), OldObject);
			OldObjectAccessor.Argument = NewObject.NewRef();
            editibility = NewObject.Editability;
            NewObject.Editability = OldObject.Editability;
		}

		protected override void Undo() {
			Assert.Same("OldObjectAccessor accesses the new object", OldObjectAccessor.Argument.Instance(), NewObject);
			OldObjectAccessor.Argument = OldObject.NewRef();
            NewObject.Editability = editibility;
        }

        public override string ToString () {
			return "ReplaceRuleComponentModification";
		}

	}

}

