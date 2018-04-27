using RuleEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewAnchor : Anchor {
        
        public NewAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public void NewAnchorFunction() {

        }

        public override string GetDescription() {
            return "A new anchor";
        }

    }

}