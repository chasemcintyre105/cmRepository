using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class TurnRuleExecutor : CommonTurnBasedRuleExecutor {

        public TurnRuleExecutor(Engine E, Player player) : base (E, player) {
            RuleType = ((TurnBasedExecutionManager) E.ExecutionManager).Turn;
        }

        protected override void _Start() {}
        protected override void _Finish() {}
		public override void StartRule() {}
		public override void FinishRule() {}
        
    }

}

