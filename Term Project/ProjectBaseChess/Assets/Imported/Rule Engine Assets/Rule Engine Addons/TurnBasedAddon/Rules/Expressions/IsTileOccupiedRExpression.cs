using System;
using RuleEngine;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {
	
	public class IsTileOccupiedRExpression : Expression {

        private BoardManager BoardManager;

        public override Type GetReturnType() {
			return typeof(BooleanValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Attributes";
		}

		public IsTileOccupiedRExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
            BoardManager = E.GetManager<BoardManager>();
        }
		
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Unit or type of unit", new Type[] { typeof(UnitObjectTypeValue), typeof(UnitObjectValue) }, "The unit with which to determine the position to examine.");
            ArgumentList.DefineArgument("Direction", typeof(DirectionValue), "The placement of the position in question relative to the unit.");
		}
        
		public void SetValues(Variable unit, DirectionValue direction) {
			Assert.Same("Variable type is UnitObjectTypeValue", unit.VariableType.GetType(), typeof(UnitObjectTypeValue));
			ArgumentList.SetArgument(0, unit);
			ArgumentList.SetArgument(1, direction);
		}

		public void SetValues(UnitObjectTypeValue unitType, DirectionValue direction) {
			ArgumentList.SetArgument(0, unitType);
			ArgumentList.SetArgument(1, direction);
		}
		
		public void SetValues(UnitObjectValue unit, DirectionValue direction) {
			ArgumentList.SetArgument(0, unit);
			ArgumentList.SetArgument(1, direction);
		}
		
		public override string GetDescription() {
			return "Determines if a position is occupied by a unit.";
		}

        public override string ToString() {
            return "IsTileOccupied";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            DirectionValue direction = argsByName["Direction"].CalculateValue<DirectionValue>(RuleExecutor);
            UnitObjectValue unit = argsByName["Unit or type of unit"].CalculateValue<UnitObjectValue>(RuleExecutor);

            Vector3 unitPosition;
            if (RuleExecutor is CommonTurnBasedRuleExecutor) {
                unitPosition = (RuleExecutor as CommonTurnBasedRuleExecutor).GetUnitPositionWithinExecutionContext(unit);
            } else {
                unitPosition = unit.GetInstance().GetOffset_TS();
            }

            Vector3 pos = unitPosition + unit.GetInstance().player.Position.GlobalToPlayerRelativeDirection(direction.value);

            Position position = BoardManager.GetPosition_TS(pos);
            if (position != null) {
                return new BooleanValue(E, position.GetUnit_TS() != null);
            } else {
                throw new RuleException("Tile is not on board", RuleException.Severity.Skipped);
            }
            
        }
    }

}