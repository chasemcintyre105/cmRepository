using System;
using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class IsCurrentPlayerExpression : Expression {

        protected PlayerManager PlayerManager;

		public override Type GetReturnType() {
			return typeof(BooleanValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Attributes";
		}

		public IsCurrentPlayerExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
            PlayerManager = E.GetManager<PlayerManager>();
        }
        
        public override void DefineArguments() {
            ArgumentList.DefineArgument("Player", typeof(PlayerObjectValue), "The player in question.");
		}
        
		public void SetValue(Value value) {
			ArgumentList.SetArgument(0, value);
		}
		
		public override string GetDescription() {
			return "Determines if a player is the current player";
		}

        public override string ToString() {
            return "IsCurrentPlayer";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            Player currentPlayer = PlayerManager.GetCurrentPlayer();
            PlayerObjectValue player = argsByOrder[0].CalculateValue<PlayerObjectValue>(RuleExecutor);

            return new BooleanValue(E, player.GetInstance().UID == currentPlayer.UID);
        }
    }

}