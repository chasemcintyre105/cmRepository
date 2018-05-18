using RuleEngine;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.TurnBased { 

	public class MovementRuleExecutor : CommonTurnBasedRuleExecutor {
		
		public Dictionary<UnitObjectValue, Vector3> CurrentPositionOfUnitInRuleCalculation;
		public Dictionary<UnitObjectValue, List<Vector3>> PossibleMovementsOfUnitVariables;

        public MovementRuleExecutor(Engine E, Player player) : base (E, player) {
            RuleType = ((TurnBasedExecutionManager) E.ExecutionManager).Movement;
        }

        protected override void _Start() {}
        protected override void _Finish() {}
		
		public override void StartRule() {
			CurrentPositionOfUnitInRuleCalculation = new Dictionary<UnitObjectValue, Vector3>();
			PossibleMovementsOfUnitVariables = new Dictionary<UnitObjectValue, List<Vector3>>();

            // Register rule variables
			foreach (Variable v in CurrentRuleContext.Rule.Variables) {
				if (v.VariableType.GetType() == typeof(UnitObjectTypeValue)) {

					UnitObjectValue unitObj = v.VariableValue.As<UnitObjectValue>();
					CurrentPositionOfUnitInRuleCalculation.Add(unitObj, unitObj.GetInstance().GetOffset_TS());
					PossibleMovementsOfUnitVariables.Add(unitObj, new List<Vector3>());

				}
			}
		}
		
		public override void FinishRule() {}

		public override Vector3 GetUnitPositionWithinExecutionContext(UnitObjectValue unit) {
            Vector3 pos;
            if (CurrentPositionOfUnitInRuleCalculation.TryGetValue(unit, out pos)) {
                return pos;
            } else {
                return unit.GetInstance().GetOffset_TS();
            }
        }

        public override void SetUnitPositionWithinExecutionContext(UnitObjectValue unit, Vector3 pos) {
            if (CurrentPositionOfUnitInRuleCalculation.ContainsKey(unit)) {
                CurrentPositionOfUnitInRuleCalculation[unit] = pos;
            } else {
                CurrentPositionOfUnitInRuleCalculation.Add(unit, pos);
            }
        }
        
    }

}

