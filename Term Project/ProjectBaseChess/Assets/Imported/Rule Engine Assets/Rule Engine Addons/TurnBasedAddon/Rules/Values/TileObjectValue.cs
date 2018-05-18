using System;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

	public class TileObjectValue : ObjectValue {
        
		public override string GetSelectionPanelCategory() {
			return "Tiles";
		}
        
		public TileObjectValue(Engine E, TileObjectTypeValue totv, Tile Instance) : base(E, totv, Instance) {
            
        }

		public Tile GetInstance() {
			return (Tile) Instance;
		}

		public override string GetDescription() {
			return "Represents a tile in the game. In this case " + Instance;
		}

        public override string ToString() {
            return "TileObjectValue: " + ((Tile) Instance).ToString();
        }

    }

}