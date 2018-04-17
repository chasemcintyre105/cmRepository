using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class CollisionRuleExecutor : CommonTurnBasedRuleExecutor {

		public Dictionary<int, Variable> variables;
		
		public CollisionRuleExecutor(Engine E, Player player) : base (E, player) {
			variables = new Dictionary<int, Variable>();

            RuleType = ((TurnBasedExecutionManager) E.ExecutionManager).Collision;
        }

        protected override void _Start() {}
        protected override void _Finish() {}
		public override void StartRule() {}
		public override void FinishRule() {}
        
        public static void VisitNonNullValueOverride(RuleExecutor RuleExecutor, RuleComponent obj) {
            CollisionRuleExecutor v = (CollisionRuleExecutor) RuleExecutor;
            if (obj is UnitObjectTypeValue) {
                v.variables.Add(v.variables.Count, new Variable(v.E, "" + v.variables.Count, (ObjectTypeValue) obj, (Value) obj));
            }
        }

    }

}

