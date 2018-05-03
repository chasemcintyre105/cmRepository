
namespace RuleEngine {

	public class ManagerConfigurationAnchor : Anchor {

        public ManagerConfigurationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that allows for the configuration of managers.";
        }

    }

}