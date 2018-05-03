using RuleEngine;

namespace RuleEngineAddons.TurnBased { 

	public class TurnBasedRuleVerifyingExecutor : RuleVerifyingExecutor, ITurnBasedRuleExecutor {
        
		public TurnBasedRuleVerifyingExecutor(Engine E) : base (E) {
		}

    }

}

