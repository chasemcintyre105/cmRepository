using RuleEngine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RuleEngineAddons.TurnBased {

    public class RegisterTurnBasedPlayersAnchor : Anchor {

        private Engine E;
        private TurnController BoardController;

        public RegisterTurnBasedPlayersAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
            E = initialiser.GetEngine();
            BoardController = E.GetController<TurnController>();
        }

        public Player AddPlayer(string Name, String Colour, Color PieceColour, PlayerPosition Position, GameObject TurnDisplay, GameObject CoordinateOrigin) {
            Player player = _AddPlayer(Name, Colour, Position, TurnDisplay, CoordinateOrigin);
            player.PieceColour = PieceColour;
            return player;
        }

        public Player AddPlayer(string Name, String Colour, Material PieceMaterial, PlayerPosition Position, GameObject TurnDisplay, GameObject CoordinateOrigin) {
            Player player = _AddPlayer(Name, Colour, Position, TurnDisplay, CoordinateOrigin);
            player.PieceMaterial = PieceMaterial;
            return player;
        }

        private Player _AddPlayer(string Name, String Colour, PlayerPosition Position, GameObject TurnDisplay, GameObject CoordinateOrigin) {
            Player newPlayer = new Player(E, Name, Position);

            newPlayer.Colour = Colour;
            newPlayer.TurnDisplay = TurnDisplay;
            newPlayer.CoordinateOrigin = CoordinateOrigin;

            Text textObj = newPlayer.TurnDisplay.GetComponent<Text>();
            if (textObj == null)
                textObj = newPlayer.TurnDisplay.GetComponentInChildren<Text>();
            if (textObj != null)
                textObj.text = newPlayer.Name;

            newPlayer.TurnDisplay.SetActive(false);

            return newPlayer;
        }

        public GameObject SetupCoordinateOriginMarker(PlayerPosition position) {
            GameObject Arrows = GameObject.Instantiate<GameObject>(E.GetController<TurnController>().XYZArrowsTemplate);
            Arrows.transform.SetParent(BoardController.GameContainer.transform);
            Arrows.transform.localScale = BoardController.BoardScale;
            Arrows.transform.localPosition = BoardController.BoardScaleModifier * position.PlayerOrigin;
            Vector3 ZInBoard = BoardController.GameContainer.transform.InverseTransformDirection(Vector3.forward);
            Vector3 YInBoard = BoardController.GameContainer.transform.InverseTransformDirection(Vector3.up);
            Arrows.transform.Rotate(ZInBoard * position.AngleFromGlobalForwardXY);
            Arrows.transform.Rotate(YInBoard * position.AngleFromGlobalForwardXZ);
            return Arrows;
        }

        public override string GetDescription() {
            return "An anchor that allows for the registration of turn based players.";
        }

    }

}