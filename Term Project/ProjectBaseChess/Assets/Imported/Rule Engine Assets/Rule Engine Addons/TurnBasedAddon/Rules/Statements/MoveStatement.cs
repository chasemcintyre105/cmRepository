using System.Collections.Generic;
using RuleEngine;
using System;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {
	
	public class MoveStatement : Statement {
        
		public override string GetSelectionPanelCategory() {
			return "Commands";
		}

		public MoveStatement(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public MoveStatement(Engine E, Variable unit, Direction d) : base(E) {
			Assert.Same("Variable has type UnitObjectTypeValue", unit.VariableType.GetType(), typeof(UnitObjectTypeValue));
			ArgumentList.SetArgument(0, unit.NewRef());
			ArgumentList.SetArgument(1, (new DirectionValue(E, d)).NewRef());
		}

		public MoveStatement(Engine E, UnitObjectValue unit, Direction d) : base(E) {
			ArgumentList.SetArgument(0, unit.NewRef());
			ArgumentList.SetArgument(1, (new DirectionValue(E, d)).NewRef());
		}
		
		public MoveStatement(Engine E, UnitObjectTypeValue unit, Direction d) : base(E) {
			ArgumentList.SetArgument(0, unit.NewRef());
			ArgumentList.SetArgument(1, (new DirectionValue(E, d)).NewRef());
		}
		
		public MoveStatement(Engine E, Variable unit, DirectionValue d) : base(E) {
			Assert.Same("Variable has type UnitObjectTypeValue", unit.VariableType.GetType(), typeof(UnitObjectTypeValue));
			ArgumentList.SetArgument(0, unit.NewRef());
			ArgumentList.SetArgument(1, d.NewRef());
		}
		
		public MoveStatement(Engine E, UnitObjectValue unit, DirectionValue d) : base(E) {
			ArgumentList.SetArgument(0, unit.NewRef());
			ArgumentList.SetArgument(1, d.NewRef());
		}
		
		public MoveStatement(Engine E, UnitObjectTypeValue unit, DirectionValue d) : base(E) {
			ArgumentList.SetArgument(0, unit.NewRef());
			ArgumentList.SetArgument(1, d.NewRef());
		}
        
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Unit or type of unit", new Type[] { typeof(UnitObjectTypeValue), typeof(UnitObjectValue) }, "A unit or a unit type to move.");
            ArgumentList.DefineArgument("Direction", typeof(DirectionValue), "The direction in which to move the specified unit or type of unit");
		}

		public override string GetDescription() {
			return "Permits a unit to move in the given direction and for a given distance.";
		}

        public override string ToString() {
            return "Move";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            UnitObjectValue unit;
            DirectionValue direction;

            switch (RuleExecutor.RuleType.ID) {
            case TurnBasedExecutionManager.MOVEMENT:
                MovementRuleExecutor v = (MovementRuleExecutor) RuleExecutor;
                
                unit = argsByName["Unit or type of unit"].CalculateValue<UnitObjectValue>(RuleExecutor);
                direction = argsByName["Direction"].CalculateValue<DirectionValue>(RuleExecutor);
                
                // Make direction player relative
                Vector3 relative = unit.GetInstance().player.Position.GlobalToPlayerRelativeDirection(direction.value);

                // Calculate the new position
                Vector3 newCurrentPosition = v.GetUnitPositionWithinExecutionContext(unit) + relative;

                // Let this new position be a possible action for this unit, if it isn't already
                List<Vector3> PossibleMovementsForThisUnit;
                if (!v.PossibleMovementsOfUnitVariables.TryGetValue(unit, out PossibleMovementsForThisUnit)) {
                    PossibleMovementsForThisUnit = new List<Vector3>();
                    v.PossibleMovementsOfUnitVariables.Add(unit, PossibleMovementsForThisUnit);
                }

                if (!PossibleMovementsForThisUnit.Contains(newCurrentPosition))
                    PossibleMovementsForThisUnit.Add(newCurrentPosition);

                // Register the new position as the new position of the unit for use later in the rule
                v.SetUnitPositionWithinExecutionContext(unit, newCurrentPosition);

                break;
            case TurnBasedExecutionManager.COLLISION:
                unit = argsByName["Unit or type of unit"].CalculateValue<UnitObjectValue>(RuleExecutor);
                direction = argsByName["Direction"].CalculateValue<DirectionValue>(RuleExecutor);
                
                Vector3 targetPosition = unit.GetInstance().GetOffset_TS() + (unit.GetInstance().player.Position.GlobalToPlayerRelativeDirection(direction.value));
                ((CollisionRuleExecutor) RuleExecutor).ApplyMod(new MoveUnitGameModification(E, unit.GetInstance(), targetPosition));

                break;
            case TurnBasedExecutionManager.TURN:
                Assert.Never("Movement not allowed in turn rules");
                break;
            }
        }
    }

}