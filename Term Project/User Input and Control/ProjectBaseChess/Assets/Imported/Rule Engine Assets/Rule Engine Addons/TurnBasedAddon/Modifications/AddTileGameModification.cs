using RuleEngine;

namespace RuleEngineAddons.TurnBased {
	
	public class AddTileGameModification : GameModification {
		
		private Tile tile;
        private bool applyEffects;
        
		public AddTileGameModification(Engine E, Tile tile, bool applyEffects = true) : base(E) {
			this.tile = tile;
            this.applyEffects = applyEffects;
        }

        public override void ValidateModifcation_TS() {
        }

        protected override void Apply() {

            // Add the position to the game
            E.GetManager<BoardManager>().TileRegistry.RegisterObject_TS(tile);

            // Signal the game with the appropriate effect
            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IAddTileEffect>(tile);
			
		}
		
		protected override void Undo() {

            // Remove the position from the game
            E.GetManager<BoardManager>().TileRegistry.UnregisterObject_TS(tile);

            // Signal the game with the appropriate effect
            if (applyEffects)
                E.EffectFactory.EnqueueNewEffect<IRemoveTileEffect>(tile);
			
		}
		
		public override string ToString () {
			return "AddTileBoardModification: " + tile.ToString();
		}
		
	}
	
}

