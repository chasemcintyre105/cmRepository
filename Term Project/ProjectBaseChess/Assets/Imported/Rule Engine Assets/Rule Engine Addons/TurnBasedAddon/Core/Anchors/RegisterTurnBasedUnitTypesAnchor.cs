using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class RegisterTurnBasedUnitTypesAnchor : Anchor {

        public RegisterTurnBasedUnitTypesAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that allows for the registration of turn based unit types.";
        }

    }

}