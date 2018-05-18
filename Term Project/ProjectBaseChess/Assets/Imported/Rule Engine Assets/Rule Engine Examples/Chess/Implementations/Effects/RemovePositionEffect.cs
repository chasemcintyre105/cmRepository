using RuleEngine;
using RuleEngineAddons.TurnBased;

namespace RuleEngineExamples.Chess {

	public class RemovePositionEffect : IRemovePositionEffect {

        public Position position;

        public override Effect Init(params object[] parameters) {
            position = (Position) parameters[0];
            return this;
        }

        public override void Apply() {
			position.GetGameObject().SetActive(false);
		}

        public override object[] GetEffectData() {
            return new object[] { position };
        }

    }

}
