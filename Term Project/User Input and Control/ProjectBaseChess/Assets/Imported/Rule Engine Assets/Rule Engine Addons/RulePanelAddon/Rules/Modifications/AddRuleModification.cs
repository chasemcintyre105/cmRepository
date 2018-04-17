using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class AddRuleModification : RuleModification {

		private Engine E;

		public RuleList AssociatedRuleList { get; private set; }

		public AddRuleModification(Engine E, RuleList TargetRules) {
			this.E = E;
			AssociatedRuleList = TargetRules;
		}

		protected override void Apply() {
			AssociatedRule = new Rule(E);
			AssociatedRuleList.Add(AssociatedRule);

            if (AssociatedRule.UID != null)
                RuleEngineController.E.RuleManager.RulesByUID.Add(AssociatedRule.UID, AssociatedRule);

        }

        protected override void Undo() {
			Assert.True("Rule is the last one in the list", AssociatedRuleList.IndexOf(AssociatedRule) == AssociatedRuleList.Count - 1);
			AssociatedRuleList.Remove(AssociatedRule);

            if (AssociatedRule.UID != null)
                RuleEngineController.E.RuleManager.RulesByUID.Remove(AssociatedRule.UID);

        }

        public override string ToString () {
			return "AddRuleModification";
		}

	}

}