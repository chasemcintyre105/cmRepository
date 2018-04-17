using System;
using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class IsPreviousPlayerExpression : Expression {

        protected PlayerManager PlayerManager;

		public override Type GetReturnType() {
			return typeof(BooleanValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Attributes";
		}

		public IsPreviousPlayerExpression(Engine E) : base(E) {
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
			return "Determines if a player is the previous player to play a turn";
		}

        public override string ToString() {
            return "IsPreviousPlayer";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            Turn PreviousTurn = PlayerManager.TurnManager.PreviousTurn;

            if (PreviousTurn == null)
                return new BooleanValue(E, false);

            PlayerObjectValue player = argsByOrder[0].CalculateValue<PlayerObjectValue>(RuleExecutor);

            return new BooleanValue(E, player.GetInstance().UID == PreviousTurn.player.UID);
        }
    }

}