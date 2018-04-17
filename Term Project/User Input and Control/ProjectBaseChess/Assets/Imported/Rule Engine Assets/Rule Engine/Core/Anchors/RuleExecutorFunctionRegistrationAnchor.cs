
namespace RuleEngine {

	public class RuleExecutorFunctionRegistrationAnchor : Anchor {

        private ExecutionManager ExecutionManager;

        public RuleExecutorFunctionRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
            ExecutionManager = initialiser.GetEngine().ExecutionManager;
        }

        public void RegisterDefaultVisitFunction<V>(ExecutionManager.VisitAction visit) where V : RuleExecutor {
            ExecutionManager.RegisterVisitFunction<RuleComponent, V>(visit);
        }

        public void RegisterStatementVisitFunction<V>(ExecutionManager.VisitAction visit) where V : RuleExecutor {
            ExecutionManager.RegisterVisitFunction<Statement, V>(visit);
        }

        public void RegisterExpressionVisitFunction<V>(ExecutionManager.VisitAction visit) where V : RuleExecutor {
            ExecutionManager.RegisterVisitFunction<Expression, V>(visit);
        }

        public void RegisterValueVisitFunction<V>(ExecutionManager.VisitAction visit) where V : RuleExecutor {
            ExecutionManager.RegisterVisitFunction<Value, V>(visit);
        }

        public void RegisterNullVisitFunction<V>(ExecutionManager.VisitAction visit) where V : RuleExecutor {
            ExecutionManager.RegisterVisitFunction<NullValue, V>(visit);
        }

        public void RegisterVoidVisitFunction<V>(ExecutionManager.VisitAction visit) where V : RuleExecutor {
            ExecutionManager.RegisterVisitFunction<VoidStatement, V>(visit);
        }

        public void RegisterSpecificVisitFunction<E, V>(ExecutionManager.VisitAction visit) where E : RuleComponent where V : RuleExecutor {
            ExecutionManager.RegisterVisitFunction<E, V>(visit);
        }

        public override string GetDescription() {
            return "An anchor that allows for the registration of rule execution handlers.";
        }

    }

}