using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class DeclareWinnerStatement : Statement {
        
		public override string GetSelectionPanelCategory() {
			return "Commands";
		}

		public DeclareWinnerStatement(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public DeclareWinnerStatement(Engine E, PlayerObjectValue p) : base(E) {
			ArgumentList.SetArgument(0, p.NewRef());
		}
        
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Player", typeof(PlayerObjectValue), "The player that will be declared as the winner.");
		}
		
		public override string GetDescription() {
			return "Declares the winner of the game.";
		}
		
		public void SetValue(Value player) {
			ArgumentList.SetArgument(0, player);
		}

        public override string ToString() {
            return "DeclareWinner";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            PlayerObjectValue argPlayer = argsByOrder[0].CalculateValue<PlayerObjectValue>(RuleExecutor);
            CommonTurnBasedRuleExecutor CEFRuleExecutor = (CommonTurnBasedRuleExecutor) RuleExecutor;

            if (!CEFRuleExecutor.playersDeclaredWinners.Contains(argPlayer.GetInstance()))
                CEFRuleExecutor.playersDeclaredWinners.Add(argPlayer.GetInstance());

        }
    }

}