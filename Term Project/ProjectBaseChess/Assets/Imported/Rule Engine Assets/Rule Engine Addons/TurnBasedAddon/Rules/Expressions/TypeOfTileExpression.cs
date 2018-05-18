using System;
using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class TypeOfTileExpression : Expression {

		public override Type GetReturnType() {
			return typeof(TileObjectTypeValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Attributes";
		}

		public TypeOfTileExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Tile", typeof(TileObjectValue), "The tile of which to determine the type.");
		}
        
        public void SetValue(Value value) {
            ArgumentList.SetArgument(0, value);
        }

        public override string GetDescription() {
			return "Gives the type of a tile.";
		}

        public override string ToString() {
            return "TypeOfTile";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            TileObjectValue argTile = argsByOrder[0].CalculateValue<TileObjectValue>(RuleExecutor);
            return argTile.TypeValue;
        }
    }

}