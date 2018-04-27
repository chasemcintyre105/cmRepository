
namespace RuleEngine {

	public class AfterStartAnchor : Anchor {

        public AfterStartAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that is processed just after the initialiser has finished.";
        }

    }

}