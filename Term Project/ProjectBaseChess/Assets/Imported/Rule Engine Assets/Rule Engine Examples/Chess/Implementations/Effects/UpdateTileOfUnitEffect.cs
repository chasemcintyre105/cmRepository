using UnityEngine;
using RuleEngine;
using RuleEngineAddons.TurnBased;

namespace RuleEngineExamples.Chess {

	public class UpdateTileOfUnitEffect : IUpdateTileOfUnitEffect {

        public Unit unit;

        public override Effect Init(params object[] parameters) {
            unit = (Unit) parameters[0];
            return this;
        }

        public override void Apply() {
			unit.GetGameObject().transform.SetParent(unit.GetGameObject().transform);
			unit.GetGameObject().transform.localPosition = -Vector3.forward;
		}

        public override object[] GetEffectData() {
            return new object[] { unit };
        }

    }

}
