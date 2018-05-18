using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class PossibleAction {

		public Rule rule;
		public Position FinalPosition;

		public PossibleAction(Rule rule, Position FinalPosition) {
			this.rule = rule;
			this.FinalPosition = FinalPosition;
		}

	}
	
}