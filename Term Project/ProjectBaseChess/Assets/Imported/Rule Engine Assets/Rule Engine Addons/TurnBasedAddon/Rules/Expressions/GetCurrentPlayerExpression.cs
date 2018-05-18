using System;
using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class GetCurrentPlayerExpression : Expression {

        protected PlayerManager PlayerManager;

        public override Type GetReturnType() {
			return typeof(PlayerObjectValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Attributes";
		}

		public GetCurrentPlayerExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
            PlayerManager = E.GetManager<PlayerManager>();
        }
		
		public override void DefineArguments() {
		}

		public override string GetDescription() {
			return "Gives the current player.";
		}

        public override string ToString() {
            return "Current Player";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            return new PlayerObjectValue(E, PlayerManager.GetCurrentPlayer());
        }
    }

}