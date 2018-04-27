using System;
using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class TileOfUnitExpression : Expression {

        private BoardManager BoardManager;

        public override Type GetReturnType() {
			return typeof(TileObjectValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Attributes";
		}

		public TileOfUnitExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
            BoardManager = E.GetManager<BoardManager>();
        }
		
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Unit", typeof(UnitObjectValue), "The unit of which to determine the tile.");
		}
        
        public void SetValue(Value value) {
            ArgumentList.SetArgument(0, value);
        }

        public void SetValue(Variable value) {
            ArgumentList.SetArgument(0, value);
        }

        public override string GetDescription() {
			return "Gives the tile of a unit.";
		}

        public override string ToString() {
            return "TileOfUnit";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            UnitObjectValue argUnit = argsByOrder[0].CalculateValue<UnitObjectValue>(RuleExecutor);
            
            Unit unit = (Unit) argUnit.Instance;
            Tile tile = BoardManager.TileRegistry.GetClosestAtOffsetWithinTolerance_TS(unit.GetOffset_TS(), BoardManager.sqrPositioningTolerance);

            return BoardManager.TileRegistry.CreateObjectValue_TS(tile);

        }
    }

}