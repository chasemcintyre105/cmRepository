using RuleEngine;
using RuleEngineAddons.TurnBased;

namespace RuleEngineExamples.Chess {

	public class StopGameObjectPlacementEffect : IStopGameObjectPlacementEffect {

        public override Effect Init(params object[] parameters) {
            return this;
        }

        public override void Apply() {
            RuleEngineController.E.GetManager<GUIManager>().StopGameObjectPlacement();
        }

        public override object[] GetEffectData() {
            return new object[] { };
        }

    }

}