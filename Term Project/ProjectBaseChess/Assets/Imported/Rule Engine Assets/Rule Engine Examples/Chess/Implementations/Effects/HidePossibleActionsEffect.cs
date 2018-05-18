using RuleEngine;
using RuleEngineAddons.RulePanel;

namespace RuleEngineExamples.Chess {

	public class HidePossibleActionsEffect : IHidePossibleActionsEffect {

        public override Effect Init(params object[] parameters) {
            return this;
        }

        public override void Apply() {
            RuleEngineController.E.GetManager<GUIManager>().HidePossibleActions();
        }

        public override object[] GetEffectData() {
            return new object[] { };
        }

    }

}