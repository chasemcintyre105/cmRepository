
namespace RuleEngine {

	public class PlayerRegistrationAnchor : Anchor {

        public PlayerRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that allows for the registration of players";
        }

    }

}