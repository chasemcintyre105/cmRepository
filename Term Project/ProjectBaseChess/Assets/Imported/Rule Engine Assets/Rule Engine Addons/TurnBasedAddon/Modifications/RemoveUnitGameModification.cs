using System.Collections.Generic;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class RemoveUnitGameModification : GameModification {

		private Unit unit;
		private LinkedListNode<Unit> nodeBefore;
        private bool applyEffects;

        // Validated information
        private Position position;
        
		public RemoveUnitGameModification(Engine E, Unit unit, bool applyEffects = true) : base(E) {
			this.unit = unit;
            this.applyEffects = applyEffects;
        }

        public override void ValidateModifcation_TS() {
			position = E.GetManager<BoardManager>().PositionRegistry.GetOnlyAtOffset_TS(unit.GetOffset_TS());
		}

		protected override void Apply() {

			nodeBefore = position.FindNodeBeforeUnit_TS(unit);
			position.RemoveUnit_TS(unit);

            // Remove the unit from the game
            //E.GetManager<BoardManager>().UnitRegistry.UnregisterObject_TS(unit);

            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IRemoveUnitEffect>(unit);

		}
		
		protected override void Undo() {

            // Add the unit to the game
            //E.GetManager<BoardManager>().UnitRegistry.RegisterObject_TS(unit);

            position.InsertNodeAfterUnit_TS(unit, nodeBefore);

			if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IAddUnitEffect>(unit, position);

		}
		
		public override string ToString () {
			return "RemoveUnitBoardModification: " + unit.ToString();
		}
        
    }

}

