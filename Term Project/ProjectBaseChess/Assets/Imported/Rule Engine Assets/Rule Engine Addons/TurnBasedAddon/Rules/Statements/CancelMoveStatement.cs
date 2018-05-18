using RuleEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public class CancelMoveStatement : Statement {
        
        public override string GetSelectionPanelCategory() {
			return "Commands";
		}

		public CancelMoveStatement(Engine E) : base(E) {
		}

		public override void DefineArguments() {
		}
		
		public override string GetDescription() {
			return "Cancels the chosen move and gives the turn back to the last player.";
		}

        public override string ToString() {
            return "Cancel Move";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            if (RuleExecutor is CommonTurnBasedRuleExecutor)
                (RuleExecutor as CommonTurnBasedRuleExecutor).MoveCancelled = true;
            else
                throw new Exception("CancelMoveStatement was executed, though not by the TurnBasedAddon. This statement must be reimplemented for use in another addon");
        }
    }

}