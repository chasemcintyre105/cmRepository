
namespace RuleEngine {

	public class RuleTypeRegistrationAnchor : Anchor {

        private RuleManager RuleManager;

        public RuleTypeRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
            RuleManager = initialiser.GetEngine().GetManager<RuleManager>();
        }
        
        public RuleType RegisterRuleType(string Name, bool NoAdditionalRulesByPlayer = false) {
            return RuleManager.RegisterRuleType(Name, NoAdditionalRulesByPlayer);
        }

        public override string GetDescription() {
            return "An anchor that allows for the registration of rule types.";
        }

    }

}