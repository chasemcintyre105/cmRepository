using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class RemoveRuleModification : RuleModification {

		public RuleList TargetRules;
		private Rule removedRule;
		private int index;

		public RemoveRuleModification(Rule AssociatedRule, RuleList TargetRules, int index) {
            this.AssociatedRule = AssociatedRule;
			this.TargetRules = TargetRules;
			this.index = index;
		}

		protected override void Apply() {
			Assert.True("Index is within bounds", index >= 0 && index < TargetRules.Count);
			removedRule = TargetRules[index];
			TargetRules.RemoveAt(index);

            if (removedRule.UID != null)
                RuleEngineController.E.RuleManager.RulesByUID.Remove(removedRule.UID);

		}

		protected override void Undo() {
			TargetRules.Insert(index, removedRule);

            if (removedRule.UID != null)
                RuleEngineController.E.RuleManager.RulesByUID.Add(removedRule.UID, removedRule);

        }

        public override string ToString () {
			return "RemoveRuleModification";
		}

	}

}
