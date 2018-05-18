using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class AddUnitGameModification : GameModification { // Done

		private Unit unit;
		private UnitType type;
		private Player player;
        private bool applyEffects;

        // Validated information
        private Position position;

		// Information used purely to derive the validated information


		public AddUnitGameModification(Engine E, Unit unit, bool applyEffects = true) : base(E) {
			this.unit = unit;
            this.applyEffects = applyEffects;
        }

        public override void ValidateModifcation_TS() {
			position = E.GetManager<BoardManager>().PositionRegistry.GetOnlyAtOffset_TS(unit.GetOffset_TS());
            Assert.NotNull("position", position);
		}

		protected override void Apply() {

            // Add the unit to the game
            //E.GetManager<BoardManager>().UnitRegistry.RegisterObject_TS(unit);

            // Add the unit to its position
            position.AddLast_TS(unit);

            // Signal the game with the appropriate effect
            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IAddUnitEffect>(unit, position);

		}

		protected override void Undo() {
			Assert.True("Un-adding unit from the location it already occupies the last position of", position.GetLastUnit_TS() == unit);

			// Remove the unit from its position
			position.RemoveLast_UndoAction_TS();

            // Remove the unit from the game
            //E.GetManager<BoardManager>().UnitRegistry.UnregisterObject_TS(unit);

            // Signal the game with the appropriate effect
            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IRemoveUnitEffect>(unit);

		}

		public override string ToString () {
			return "AddUnitBoardModification: " + unit.ToString();
		}

	}

}

