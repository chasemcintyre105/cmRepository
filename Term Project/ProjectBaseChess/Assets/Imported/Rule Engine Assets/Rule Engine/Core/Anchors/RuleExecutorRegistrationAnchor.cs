
namespace RuleEngine {

	public class RuleExecutorRegistrationAnchor : Anchor {

        private ExecutionManager ExecutionManager;

        public RuleExecutorRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
            ExecutionManager = initialiser.GetEngine().ExecutionManager;
        }

        public void RegisterRuleExecutorType<V>() where V : RuleExecutor {
            ExecutionManager.RegisterRuleExecutorType<V>();
        }

        public override string GetDescription() {
            return "An anchor that allows for the registration of rule executors.";
        }

    }

}