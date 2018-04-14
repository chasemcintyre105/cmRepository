using RuleEngine;
using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class MoveAndReturnUnitEffect : IMoveAndReturnUnitEffect {

        public Engine E;
        public Unit unit;
        public Vector3 currentOffset;
        public Vector3 targetOffset;

        public override Effect Init(params object[] parameters) {
            E = (Engine) parameters[0];
            unit = (Unit) parameters[1];
            currentOffset = unit.GetOffset_TS();
            targetOffset = (Vector3) parameters[2];
            return this;
        }

        public override void Apply() {
            // No effect since movements are now instant
		}

        public override object[] GetEffectData() {
            return new object[] { unit, currentOffset, targetOffset };
        }

    }

}
