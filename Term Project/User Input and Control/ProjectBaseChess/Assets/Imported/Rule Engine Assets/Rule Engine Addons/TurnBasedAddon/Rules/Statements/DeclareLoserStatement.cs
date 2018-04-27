using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class DeclareLoserStatement : Statement {
        
		public override string GetSelectionPanelCategory() {
			return "Commands";
		}

		public DeclareLoserStatement(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public DeclareLoserStatement(Engine E, PlayerObjectValue p) : base(E) {
			ArgumentList.SetArgument(0, p.NewRef());
		}
        
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Player", typeof(PlayerObjectValue), "The player that will be declared as the loser.");
		}
		
		public override string GetDescription() {
			return "Declares the loser of the game.";
		}
		
		public void SetValue(Value player) {
			ArgumentList.SetArgument(0, player);
		}

        public override string ToString() {
            return "DeclareLoser";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            PlayerObjectValue argPlayer = argsByOrder[0].CalculateValue<PlayerObjectValue>(RuleExecutor);
            CommonTurnBasedRuleExecutor CEFRuleExecutor = (CommonTurnBasedRuleExecutor) RuleExecutor;

            if (!CEFRuleExecutor.playersDeclaredLosers.Contains(argPlayer.GetInstance()))
                CEFRuleExecutor.playersDeclaredLosers.Add(argPlayer.GetInstance());

        }
    }

}