using RuleEngine;

namespace RuleEngineAddons.TurnBased {

	public class PlayerType : IObjectType {
        
		public PlayerType() {
		}

        public string GetID() {
            return "Player type";
        }

    }
	
}