using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class AddPositionGameModification : GameModification {
		
		private Position position;
        private bool applyEffects;
        
		public AddPositionGameModification(Engine E, Position position, bool applyEffects = true) : base(E) {
			this.position = position;
            this.applyEffects = applyEffects;
		}

        public override void ValidateModifcation_TS() {
        }

        protected override void Apply() {

            // Add the position to the game
            E.GetManager<BoardManager>().PositionRegistry.RegisterObject_TS(position);
			
			// Signal the game with the appropriate effect
            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IAddPositionEffect>(position);

		}
		
		protected override void Undo() {

            // Remove the position from the game
            E.GetManager<BoardManager>().PositionRegistry.UnregisterObject_TS(position);
			
			// Signal the game with the appropriate effect
            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IRemovePositionEffect>(position);
			
		}
		
		public override string ToString () {
			return "AddPositionBoardModification: " + position.ToString();
		}

    }
	
}

