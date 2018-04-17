using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class RemoveUnitStatement : Statement {
        
		public override string GetSelectionPanelCategory() {
			return "Commands";
		}

		public RemoveUnitStatement(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public RemoveUnitStatement(Engine E, Variable o) : base(E) {
			ArgumentList.SetArgument(0, o.NewRef());
		}
		
		public RemoveUnitStatement(Engine E, UnitObjectValue o) : base(E) {
			ArgumentList.SetArgument(0, o.NewRef());
		}

		public override void DefineArguments() {
            ArgumentList.DefineArgument("Unit", typeof(UnitObjectValue), "The unit to remove from the game");
		}
		
		public override string GetDescription() {
			return "Removes a unit from the game.";
		}

        public override string ToString() {
            return "Remove";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            UnitObjectValue unit = argsByOrder[0].CalculateValue<UnitObjectValue>(RuleExecutor);
            ((CommonTurnBasedRuleExecutor) RuleExecutor).ApplyMod(new RemoveUnitGameModification(E, unit.GetInstance()));
        }
    }

}