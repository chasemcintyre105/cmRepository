
namespace RuleEngine {

	public class CompileRulesJob : ThreadedJob {

		public ExecutionManager ExecutionManager;

		public CompileRulesJob() : base ("CompileRulesJob") { }

		protected override void OnStart() {
			SelectiveDebug.StartTimer("CompileRules");

            ExecutionManager.CompileRules();

		}

		protected override void OnFinish() {

			SelectiveDebug.StopTimer("CompileRules");

        }

    }

}