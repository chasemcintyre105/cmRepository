using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class RegisterTurnBasedPositionTypesAnchor : Anchor {

        public RegisterTurnBasedPositionTypesAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that allows for the registration of turn based position types.";
        }

    }

}