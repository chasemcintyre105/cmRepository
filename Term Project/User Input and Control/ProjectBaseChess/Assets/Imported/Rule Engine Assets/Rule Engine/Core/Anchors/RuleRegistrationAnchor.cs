
namespace RuleEngine {

	public class RuleRegistrationAnchor : Anchor {

        public RuleRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that allows for the registration of rules.";
        }

    }

}