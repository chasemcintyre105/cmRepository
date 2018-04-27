
namespace RuleEngine {

	public class RuleExecutionAnchor : Anchor {

        public RuleExecutionAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that allows for the first time execution of certain sets of rules.";
        }

    }

}