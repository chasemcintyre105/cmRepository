using RuleEngine;

namespace RuleEngineAddons.TurnBased {

	public abstract class GameModification : Modification {

		protected Engine E;

		public GameModification(Engine E) {
			this.E = E;
		}

		// Thread safety must be considered when implementing this method
        // The only place this function is called is in the BoardManager
		public abstract void ValidateModifcation_TS();

	}

}
