using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class RegisterTurnBasedUnitsAnchor : Anchor {

        public RegisterTurnBasedUnitsAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that allows for the registration of turn based units.";
        }

    }

}