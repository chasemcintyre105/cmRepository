using RuleEngine;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public class TurnController : IController {

        public GameObject GameContainer;
        public GameObject XYZArrowsTemplate;

        public bool GameAcceptingUserInput = false;
        public float PositioningTolerance = 0.05f;

        public float BoardScaleModifier = 45;
		public Vector3 BoardScale;
        public PositionType PositionTypePlacemarker;

        public override void Preinit() {
            Assert.NotNull("OrganisationalObjects.GameContainer", GameContainer);
            GameAcceptingUserInput = false;
            BoardScale = new Vector3(BoardScaleModifier, BoardScaleModifier, BoardScaleModifier);
        }

        public override void Init() {
        }

    }

}