using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class RemoveTileGameModification : GameModification {
		
		private Tile tile;
        private bool applyEffects;
        private BoardManager BoardManager;

        public RemoveTileGameModification(Engine E, Tile tile, bool applyEffects = true) : base(E) {
			this.tile = tile;
            this.applyEffects = applyEffects;
            BoardManager = E.GetManager<BoardManager>();
        }

        public override void ValidateModifcation_TS() {
			
		}
		
		protected override void Apply() {

            // Remove the position to the game
            BoardManager.TileRegistry.UnregisterObject_TS(tile);

            // Signal the game with the appropriate effect
            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IRemoveTileEffect>(tile);
			
		}
		
		protected override void Undo() {

            // Add the position from the game
            BoardManager.TileRegistry.RegisterObject_TS(tile);

            // Signal the game with the appropriate effect
            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IAddTileEffect>(tile);
			
		}
		
		public override string ToString () {
			return "RemoveTileBoardModification: " + tile.ToString();
		}
		
	}
	
}

