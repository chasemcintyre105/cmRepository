using RuleEngine;
using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class AddPositionEffect : IAddPositionEffect {

        public Position position;

        public override Effect Init(params object[] parameters) {
            position = (Position) parameters[0];
            return this;
        }

        public override void Apply() {
			Assert.NotNull("The position has already been configured", position.GetBoardObjectAttachment());

            TurnController BoardController = RuleEngineController.E.GetController<TurnController>();

            position.GetGameObject().transform.localPosition = BoardController.BoardScaleModifier * position.GetOffset_TS() + 3.2f * Vector3.forward;
			position.GetGameObject().transform.localScale = BoardController.BoardScale;
			position.GetGameObject().transform.localRotation = Quaternion.identity;

		}

        public override object[] GetEffectData() {
            return new object[] { position };
        }

    }

}
