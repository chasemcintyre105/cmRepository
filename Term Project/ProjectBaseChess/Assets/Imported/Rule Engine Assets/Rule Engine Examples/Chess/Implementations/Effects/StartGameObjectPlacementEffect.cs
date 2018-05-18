using RuleEngine;
using RuleEngineAddons.TurnBased;

namespace RuleEngineExamples.Chess {

	public class StartGameObjectPlacementEffect : IStartGameObjectPlacementEffect {

        private TileType TileType;

        public override Effect Init(params object[] parameters) {
            TileType = (TileType) parameters[0];
            return this;
        }

        public override void Apply() {
            RuleEngineController.E.GetManager<GUIManager>().StartGameObjectPlacement(TileType);
        }

        public override object[] GetEffectData() {
            return new object[] { };
        }

    }

}