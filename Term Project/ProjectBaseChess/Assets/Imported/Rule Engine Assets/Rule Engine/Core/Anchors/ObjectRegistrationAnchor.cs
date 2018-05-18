
namespace RuleEngine {

	public class ObjectRegistrationAnchor : Anchor {

        public ObjectRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that allows for the registration of objects.";
        }

    }

}