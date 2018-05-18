using RuleEngine;

namespace RuleEngineAddons.TurnBased {

    public class RegisterTurnBasedGroupingUnitTypesAnchor : Anchor {

        public RegisterTurnBasedGroupingUnitTypesAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public override string GetDescription() {
            return "An anchor that allows for the registration of grouping unit types.";
        }

    }

}