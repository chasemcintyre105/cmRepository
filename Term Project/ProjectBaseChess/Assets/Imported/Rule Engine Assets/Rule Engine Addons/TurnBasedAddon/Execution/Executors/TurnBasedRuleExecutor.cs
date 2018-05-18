using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public abstract class TurnBasedRuleExecutor : RuleExecutor, ITurnBasedRuleExecutor {

        public UnitType CurrentUnitType;

        public TurnBasedRuleExecutor(Engine E) : base (E) {}
        
    }

}

