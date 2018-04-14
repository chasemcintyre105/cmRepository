using RuleEngine;
using RuleEngineAddons.TurnBased;

namespace RuleEngineExamples.Chess {

	public class RemoveTileEffect : IRemoveTileEffect {

        public Tile tile;

        public override Effect Init(params object[] parameters) {
            tile = (Tile) parameters[0];
            return this;
        }

        public override void Apply() {
			tile.GetGameObject().SetActive(false);
		}

        public override object[] GetEffectData() {
            return new object[] { tile };
        }

    }

}
