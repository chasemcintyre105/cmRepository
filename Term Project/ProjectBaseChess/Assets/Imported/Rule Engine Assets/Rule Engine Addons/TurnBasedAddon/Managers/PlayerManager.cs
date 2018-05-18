using System.Collections.Generic;
using RuleEngine;
using RuleEngineAddons.RulePanel;

namespace RuleEngineAddons.TurnBased {

    public class PlayerManager : IManager {
        
        public Player FirstPlayer { get; private set; }
        public List<Player> Players { get; private set; }
		public Dictionary<PlayerPosition, Player> PlayersByPosition { get; private set; }
		public Dictionary<string, Player> PlayersByName { get; private set; }
		public Dictionary<int, Player> PlayersByUID { get; private set; }

        public PlayerType PlayerType { get; private set; }
        public PlayerObjectTypeValue PlayerObjectTypeValue { get; private set; }

        public TurnManager TurnManager;
        
        public override void Preinit() {

            Players = new List<Player>();
            PlayersByUID = new Dictionary<int, Player>();
            PlayersByName = new Dictionary<string, Player>();
            PlayersByPosition = new Dictionary<PlayerPosition, Player>();

            PlayerType = new PlayerType();

        }

        public override void Init() {
			PlayerObjectTypeValue = new PlayerObjectTypeValue(E, PlayerType);
            TurnManager = E.GetManager<TurnManager>();
        }

        public virtual bool IsMultiplayer() {
            return Players.Count > 1;
        }

        public virtual void RegisterPlayer(Player player) {

            Players.Add(player);
            PlayersByUID.Add(player.UID, player);
            PlayersByName.Add(player.Name, player);
            PlayersByPosition.Add(player.Position, player);

            if (FirstPlayer == null)
                FirstPlayer = player;

            E.GetManager<RulePanelManager>().RegisterSelectableObject(new PlayerObjectValue(E, player));

        }

        public virtual Player GetCurrentPlayer() {
            Player currentPlayer = TurnManager.CurrentTurn.player;

            if (currentPlayer == null)
                return FirstPlayer;
            else
                return currentPlayer;
        }

    }

}