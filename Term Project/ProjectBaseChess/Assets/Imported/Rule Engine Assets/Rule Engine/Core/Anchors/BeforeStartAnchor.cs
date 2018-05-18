
namespace RuleEngine {

	public class BeforeStartAnchor : Anchor {

        public BeforeStartAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that is processed just before the initialiser finishes.";
        }

    }

}