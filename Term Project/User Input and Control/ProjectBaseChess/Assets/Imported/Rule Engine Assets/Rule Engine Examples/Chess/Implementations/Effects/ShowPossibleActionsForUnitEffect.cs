using RuleEngine;
using RuleEngineAddons.RulePanel;
using RuleEngineAddons.TurnBased;

namespace RuleEngineExamples.Chess {

	public class ShowPossibleActionsForUnitEffect : IShowPossibleActionsForUnitEffect {

        private Unit unit;

        public override Effect Init(params object[] parameters) {
            unit = (Unit) parameters[0];
            return this;
        }

        public override void Apply() {
            RuleEngineController.E.GetManager<GUIManager>().ShowPossibleActionsForUnit(unit);
        }

        public override object[] GetEffectData() {
            return new object[] { };
        }

    }

}