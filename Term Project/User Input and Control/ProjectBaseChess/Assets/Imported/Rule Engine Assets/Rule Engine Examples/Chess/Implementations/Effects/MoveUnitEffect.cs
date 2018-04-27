using RuleEngine;
using RuleEngineExamples.Chess;
using RuleEngineAddons.TurnBased;

namespace RuleEngineExamples.Chess {

    public class MoveUnitEffect : IMoveUnitEffect {

        public Unit unit;
		public Position destinationPosition;

        public override Effect Init(params object[] parameters) {
            unit = (Unit) parameters[0];
            destinationPosition = (Position) parameters[1];
            return this;
        }

        public override void Apply() {
			MoveObjectAttachment.GetMoveObject(unit.GetGameObject()).Engage(unit, destinationPosition.GetGameObject());
		}

        public override object[] GetEffectData() {
            return new object[] { unit, destinationPosition };
        }

    }

}
