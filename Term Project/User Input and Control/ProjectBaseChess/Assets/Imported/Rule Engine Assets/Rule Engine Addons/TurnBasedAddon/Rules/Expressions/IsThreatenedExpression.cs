using System;
using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class IsThreatenedExpression : Expression {

        private BoardManager BoardManager;

        public override Type GetReturnType() {
			return typeof(BooleanValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Attributes";
		}

		public IsThreatenedExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
            BoardManager = E.GetManager<BoardManager>();
        }
		
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Unit or type of unit", new Type[] { typeof(UnitObjectTypeValue), typeof(UnitObjectValue) }, "If another unit may move to the place of this unit this turn, this statement will give true.");
		}
        
		public void SetValue(Variable var) {
			Assert.Same("Type of variable is UnitObjectTypeValue", var.VariableType.GetType(), typeof(UnitObjectTypeValue));
			ArgumentList.SetArgument(0, var);
		}
		
		public void SetValue(UnitObjectTypeValue unitType) {
			ArgumentList.SetArgument(0, unitType);
		}
		
		public void SetValue(UnitObjectValue unit) {
			ArgumentList.SetArgument(0, unit);
		}
		
		public override string GetDescription() {
			return "Determines if a unit could be the target of another unit this turn";
		}

        public override string ToString() {
            return "IsThreatened";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            UnitObjectValue givenUnit = argsByOrder[0].CalculateValue<UnitObjectValue>(RuleExecutor);

            bool result = false;

            foreach (Unit unit in BoardManager.UnitRegistry.AllObjectsEnumerable_TS()) {
                if (unit.player != givenUnit.GetInstance().player) {
                    foreach (PossibleAction action in unit.GetPossibleActions_TS()) {
                        if (action.FinalPosition.GetOffset_TS() == givenUnit.GetInstance().GetOffset_TS()) {
                            result = true;
                            break;
                        }
                    }
                }
                if (result)
                    break;
            }

            return new BooleanValue(E, result);
        }
    }

}