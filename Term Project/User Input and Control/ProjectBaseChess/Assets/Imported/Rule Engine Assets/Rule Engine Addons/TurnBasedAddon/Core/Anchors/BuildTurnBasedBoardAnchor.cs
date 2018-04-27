using RuleEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

	public class BuildTurnBasedBoardAnchor : Anchor {

        private Engine E;

        public BuildTurnBasedBoardAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
            E = initialiser.GetEngine();
        }

        public void PlaceSquareGridOfTiles(Vector2 offset, Vector2 dimensions, TileType TileType, Dictionary<Vector2, Tile> tiles) {
            Assert.NotNull("Tiles is not null", tiles);

            Vector2 pos;
            Tile newTile;

            for (float x = offset.x; x < offset.x + dimensions.x; x++) {
                for (float y = offset.y; y < offset.y + dimensions.y; y++) {
                    pos = new Vector2(x, y);
                    new TileCreator_TS(E, TileType, pos).Finalise(out newTile);
                    tiles.Add(pos, newTile);
                }
            }

        }

        public void PlaceSquareGridOfTiles(Vector2 offset, Vector2 dimensions, Func<Vector2, TileType> GetTileType, Dictionary<Vector2, Tile> tiles) {
            Assert.NotNull("Tiles is not null", tiles);

            Vector2 pos;
            Tile newTile;

            for (float x = offset.x; x < offset.x + dimensions.x; x++) {
                for (float y = offset.y; y < offset.y + dimensions.y; y++) {
                    pos = new Vector2(x, y);
                    new TileCreator_TS(E, GetTileType(pos), pos).Finalise(out newTile);
                    tiles.Add(pos, newTile);
                }
            }

        }

        public void PlaceSingleTile(Vector2 pos, TileType TileType, Dictionary<Vector2, Tile> tiles) {
            Assert.NotNull("Tiles is not null", tiles);

            Tile tile;
            new TileCreator_TS(E, TileType, pos).Finalise(out tile);
            tiles.Add(pos, tile);

        }

        public void PlaceSingleTile(Vector2 pos, Func<Vector2, TileType> GetTileType, Dictionary<Vector2, Tile> tiles) {
            Assert.NotNull("Tiles is not null", tiles);

            Tile tile;
            new TileCreator_TS(E, GetTileType(pos), pos).Finalise(out tile);
            tiles.Add(pos, tile);

        }

        public void PlaceSingleTile(Vector2 pos, TileType TileType, Dictionary<Vector2, Tile> tiles, out Tile tile) {
            Assert.NotNull("Tiles is not null", tiles);

            new TileCreator_TS(E, TileType, pos).Finalise(out tile);
            tiles.Add(pos, tile);

        }

        public void PlaceSingleTile(Vector2 pos, Func<Vector2, TileType> GetTileType, Dictionary<Vector2, Tile> tiles, out Tile tile) {
            Assert.NotNull("Tiles is not null", tiles);

            new TileCreator_TS(E, GetTileType(pos), pos).Finalise(out tile);
            tiles.Add(pos, tile);

        }

        public TileType GetRandomTileType() {
            return GetRandomTileType(Vector2.zero);
        }

        public TileType GetRandomTileType(Vector2 pos) {
            Assert.True("Rarity total is set", initialiser.GetEngine().Has_TS("RarityTotal"));
            WeightedRandomSelection rs = new WeightedRandomSelection(initialiser.GetEngine().Get_TS<float>("RarityTotal"));
            foreach (TileType type in E.GetManager<BoardManager>().TileRegistry.AllObjectTypesEnumerable_TS()) {
                if (type.Rarity > 0 && rs.Check(type.Rarity))
                    return type;
            }
            Assert.Never("WeightedRandomSelection has failed to make a selection.");
            return null;
        }

        public override string GetDescription() {
            return "An anchor that allows for the construction of a turn based board.";
        }

    }

}