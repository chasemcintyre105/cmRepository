using System;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class PlayerObjectValue : ObjectValue {
        
		public override string GetSelectionPanelCategory() {
			return "Players";
		}
        
		public PlayerObjectValue(Engine E, Player Instance) : base(E, E.GetManager<PlayerManager>().PlayerObjectTypeValue, Instance) {
		}
		
		public Player GetInstance() {
			return (Player) Instance;
		}

		public override string GetDescription() {
			return "Represents the player " + Instance.ToString();
		}

        public override string ToString() {
            return "PlayerObjectValue: " + ((Player) Instance).Name;
        }

    }

}