using System;
using System.Collections.Generic;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class PlayerObjectTypeValue : ObjectTypeValue {
        
		public override string GetSelectionPanelCategory() {
			return "Player";
		}
        
		public override bool IsEqualTo (Value value) {
            return value.GetType() != GetType();
		}

		public PlayerObjectTypeValue(Engine E, PlayerType type) : base(E, type) {
		}
		
		public override string GetDescription() {
			return "Represents players in the game";
		}

        public override string ToString() {
            return "PlayerObjectTypeValue";
        }

        public override List<ObjectValue> GetAllInstances() {
            List<ObjectValue> list = new List<ObjectValue>();

            foreach (Player player in E.GetManager<PlayerManager>().Players)
                list.Add(new PlayerObjectValue(E, player));

            return list;
        }

    }

}