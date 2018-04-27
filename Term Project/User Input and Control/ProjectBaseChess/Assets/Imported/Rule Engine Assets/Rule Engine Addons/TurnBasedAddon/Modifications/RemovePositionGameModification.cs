using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class RemovePositionGameModification : GameModification {
		
		private Position position;
        private bool applyEffects;
        private BoardManager BoardManager;

        public RemovePositionGameModification(Engine E, Position position, bool applyEffects = true) : base(E) {
			this.position = position;
            this.applyEffects = applyEffects;
            BoardManager = E.GetManager<BoardManager>();
        }

        public override void ValidateModifcation_TS() {
			
		}
		
		protected override void Apply() {

            // Remove the position to the game
            BoardManager.PositionRegistry.UnregisterObject_TS(position);

            // Signal the game with the appropriate effect
            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IRemovePositionEffect>(position);
			
		}
		
		protected override void Undo() {

            // Add the position from the game
            BoardManager.PositionRegistry.RegisterObject_TS(position);

            // Signal the game with the appropriate effect
            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IAddPositionEffect>(position);
			
		}
		
		public override string ToString () {
			return "RemovePositionBoardModification: " + position.ToString();
		}
		
	}
	
}

