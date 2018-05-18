using UnityEngine;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class MoveUnitGameModification : GameModification {

		private Unit unit;
        private bool applyEffects;

        // Validated information
        private Position originalPosition;
		private Position targetPosition;

		// Information used purely to derive the validated information
		private Vector3 targetOffset;

		public MoveUnitGameModification(Engine E, Unit unit, Vector3 targetOffset, bool applyEffects = true) : base(E) {
			this.unit = unit;
			this.targetOffset = targetOffset;
            this.applyEffects = applyEffects;
		}
		
		public override void ValidateModifcation_TS() {
			originalPosition = E.GetManager<BoardManager>().PositionRegistry.GetOnlyAtOffset_TS(unit.GetOffset_TS());
			targetPosition = E.GetManager<BoardManager>().PositionRegistry.GetOnlyAtOffset_TS(targetOffset);
		}

		protected override void Apply() {
			Assert.True("Original location contains unit", originalPosition.HasUnit_TS(unit));

            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IMoveUnitEffect>(unit, targetPosition);

			// Move the unit from its original position to its target position
			originalPosition.RemoveUnit_TS(unit);
			targetPosition.AddLast_TS(unit);

		}

		protected override void Undo() {
			Assert.Same("Target location contains unit in last position", targetPosition.GetLastUnit_TS(), unit);

            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IMoveUnitEffect>(unit, originalPosition);

			// Move the unit back to its original position
			targetPosition.RemoveLast_UndoAction_TS();
            unit.UndoLastMove_TS();
            originalPosition.AddFirst_UndoAction_TS(unit);

			Assert.Same("Undone position and last recorded position match", unit.GetOffset_TS(), originalPosition.GetOffset_TS());
		}
		
		public override string ToString () {
			return "MoveUnitBoardModification: " + unit.type.ToString() + " from " + originalPosition.ToString() + " to " + targetPosition.ToString();
		}

	}

}
