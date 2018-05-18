using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class TileObjectRegistry : BoardObjectRegistry<Tile, TileType, TileObjectValue, TileObjectTypeValue> {

        private static ObjectValueCreator CreateTileValue = delegate (Engine E, Tile o, TileObjectTypeValue otv) {
            return new TileObjectValue(E, otv, o);
        };

        private static ObjectTypeValueCreator CreateTileTypeValue = delegate (Engine E, TileType ot) {
            return new TileObjectTypeValue(E, ot);
        };

        public TileObjectRegistry(Engine E) : base(E, CreateTileValue, CreateTileTypeValue, true) {
        }
        
	}

}
