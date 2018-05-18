using RuleEngine;
using RuleEngineAddons.TurnBased;

namespace RuleEngineExamples.Chess {

    public class AddTileEffect : IAddTileEffect {

        public Tile tile;

        public override Effect Init(params object[] parameters) {
            tile = (Tile) parameters[0];
            return this;
        }

        public override void Apply() {
			Assert.NotNull("The tile has already been configured", tile.GetBoardObjectAttachment());

			tile.GetGameObject().transform.localPosition = RuleEngineController.E.GetController<TurnController>().BoardScaleModifier * tile.GetOffset_TS();
			tile.GetGameObject().SetActive(true);

		}

        public override object[] GetEffectData() {
            return new object[] { tile };
        }

    }

}
