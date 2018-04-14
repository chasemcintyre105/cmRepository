using System;
using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {
	
	public class ReplaceUnitStatement : Statement {
        
		public override string GetSelectionPanelCategory() {
			return "Commands";
		}

		public ReplaceUnitStatement(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public ReplaceUnitStatement(Engine E, Variable o, UnitObjectTypeValue type) : base(E) {
			ArgumentList.SetArgument(0, o.NewRef());
			ArgumentList.SetArgument(1, type.NewRef());
		}
		
		public ReplaceUnitStatement(Engine E, UnitObjectValue o, UnitObjectTypeValue type) : base(E) {
			ArgumentList.SetArgument(0, o.NewRef());
			ArgumentList.SetArgument(1, type.NewRef());
		}
        
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Unit or type of unit", new Type[] { typeof(UnitObjectTypeValue), typeof(UnitObjectValue) }, "The unit or unit type to replace.");
            ArgumentList.DefineArgument("Unit type", typeof(UnitObjectTypeValue), "The unit type to replace the unit with", false);
		}
		
		public override string GetDescription() {
			return "Replaces one unit with another.";
		}

        public override string ToString() {
            return "Replace";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            UnitObjectValue unit = argsByName["Unit or type of unit"].CalculateValue<UnitObjectValue>(RuleExecutor);
            UnitObjectTypeValue unitType = argsByName["Unit type"].CalculateValue<UnitObjectTypeValue>(RuleExecutor);

            ((CommonTurnBasedRuleExecutor) RuleExecutor).ApplyMod(new RemoveUnitGameModification(E, unit.GetInstance()));

            // Potential problem of excess objects created here if the rule is undone and redone
            new UnitCreator_TS(E, unit.GetInstance().GetOffset_TS(), unit.GetInstance().player, (UnitType) unitType.TypeInstance).Finalise();

        }
    }

}