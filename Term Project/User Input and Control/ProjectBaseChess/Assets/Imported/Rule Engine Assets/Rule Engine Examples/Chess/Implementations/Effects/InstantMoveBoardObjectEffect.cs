using UnityEngine;
using RuleEngine;
using RuleEngineAddons.TurnBased;

namespace RuleEngineExamples.Chess {

	public class InstantMoveBoardObjectEffect : IInstantMoveBoardObjectEffect {

        public Engine E;
        public IBoardObject obj;
		public Vector3 destination;

        public override Effect Init(params object[] parameters) {
            E = (Engine) parameters[0];
            obj = (IBoardObject) parameters[1];
            destination = (Vector3) parameters[2];
            return this;
        }

        public override void Apply() {
            obj.SetOffset_TS(destination);
		}

        public override object[] GetEffectData() {
            return new object[] { obj, destination };
        }

    }

}
