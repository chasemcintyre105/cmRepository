using RuleEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class ChessSettingsController : IController {
        
        public override void Preinit() {
            TypeFunctions.CheckAllFieldsNotNull(BoardObjects);
            TypeFunctions.CheckAllFieldsNotNull(BoardTemplates);
        }

        public override void Init() {
            BoardObjects.WinnerPanel.SetActive(true);
        }
        
        [Serializable]
		public class BoardObjectContainer {
			public GameObject Player1TurnDisplay;
			public GameObject Player2TurnDisplay;
			public GameObject WinnerPanel;
        }
        public BoardObjectContainer BoardObjects;

        [Serializable]
        public class TemplateProfile {
            public string Name;
            public GameObject Template;
        }

        [Serializable]
        public class PlayerColourProfile {
            public string Name;
            public Color Colour;
            public Material Material;
        }

        [Serializable]
        public class TileProfile {
            public string ID;
            public ChessTileType Type;
            public float Rarity = 1;
            public bool AllowRandomRotation;
            public float MinimumHeight;
            public float MaximumHeight;
            public GameObject Template;
        }

        [Serializable]
        public class TileStates {
            public Color TileTint1;
            public Color TileTint2;
            public Material TileHighlighted;
        }

        [Serializable]
		public class BoardTemplateContainer {
            public List<TemplateProfile> TemplateProfiles = new List<TemplateProfile>();
            public List<PlayerColourProfile> PlayerColourProfiles = new List<PlayerColourProfile>();
            public List<TileProfile> TileTemplates = new List<TileProfile>();
            public TileStates tileStates;
            //public GameObject AnyUnitTemplate;
            public GameObject PositionTemplate;
			public Material UnpossibleActionMaterial;
			public Material PossibleActionMaterial;
		}
		public BoardTemplateContainer BoardTemplates;
        
	}

}
