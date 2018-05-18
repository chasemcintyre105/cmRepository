
namespace RuleEngine {

	public class ObjectTypeRegistrationAnchor : Anchor {

        public ObjectTypeRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that allows for the registration of object types.";
        }

    }

}