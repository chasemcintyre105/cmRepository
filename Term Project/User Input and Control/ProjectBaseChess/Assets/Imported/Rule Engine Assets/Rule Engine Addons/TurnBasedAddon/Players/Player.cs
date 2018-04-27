using UnityEngine;
using RuleEngine;
using System;

namespace RuleEngineAddons.TurnBased {

	public class Player : IObject {

		private static int playerCounter = 0;

		public readonly string Name;
		public readonly int UID;
		public readonly PlayerPosition Position;
        public readonly PlayerType type;
		public string Colour;

        public Color PieceColour;
        public Material PieceMaterial;

		public GameObject TurnDisplay;
        public GameObject CoordinateOrigin;
        
		public Player(Engine E, string Name, PlayerPosition Position) {
			this.Name = Name;
			this.Position = Position;
			this.UID = playerCounter++;

            PlayerManager PlayerManager = E.GetManager<PlayerManager>();

            type = PlayerManager.PlayerType;
            PlayerManager.RegisterPlayer(this);

		}

        public int GetID() {
            return UID;
        }

        public IObjectType GetObjectType() {
            return type;
        }

        public override string ToString() {
			return Name + " (" + Colour + ")";
		}

    }
	
}