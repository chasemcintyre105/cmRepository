using RuleEngine;
using RuleEngineAddons.TurnBased;

namespace RuleEngineExamples.Chess {

	public class RemoveUnitEffect : IRemoveUnitEffect {

        public Unit unit;

        public override Effect Init(params object[] parameters) {
            unit = (Unit) parameters[0];
            return this;
        }

        public override void Apply() {
			unit.GetGameObject().SetActive(false);
		}

        public override object[] GetEffectData() {
            return new object[] { unit };
        }

    }

}
