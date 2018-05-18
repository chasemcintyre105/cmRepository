using RuleEngine;
using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class AddUnitEffect : IAddUnitEffect {

        public Unit unit;
        public Position position;

        public override Effect Init(params object[] parameters) {
            unit = (Unit) parameters[0];
            position = (Position) parameters[1];
            return this;
        }

        public override void Apply() {
			Assert.NotNull("The unit has already been configured", unit.GetBoardObjectAttachment());

			unit.GetGameObject().transform.SetParent(position.GetGameObject().transform);
			unit.GetGameObject().transform.localPosition = (2 / RuleEngineController.E.GetController<TurnController>().BoardScaleModifier) * Vector3.back; // Bring the unit up just a tiny bit to escape the game positions
			unit.GetGameObject().transform.localRotation = Quaternion.identity;

			unit.GetGameObject().SetActive(true);
		}

        public override object[] GetEffectData() {
            return new object[] { unit, position };
        }

    }

}
