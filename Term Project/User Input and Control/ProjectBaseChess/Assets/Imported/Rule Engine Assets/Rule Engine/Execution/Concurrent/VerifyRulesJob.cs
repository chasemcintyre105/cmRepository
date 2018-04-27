
namespace RuleEngine {

	public class VerifyRulesJob : ThreadedJob {

		public Engine E;
		public RuleVerifyingExecutor RuleExecutor;

		public VerifyRulesJob() : base ("VerifyRulesJob") { }

		protected override void OnStart() {
			SelectiveDebug.LogRuleExecutor("Starting VerifyRules Job");
			SelectiveDebug.StartTimer("VerifyRules");

			RuleExecutor.ObjectCount = 0;
			RuleExecutor.ExecuteRulesForEachRuleType();
			
			for (int i = RuleExecutor.VoidsToRemove.Count - 1; i >= 0; i--) { // Do it in reverse order so that induces are not changed before they are used
				ArgumentAccessor a = RuleExecutor.VoidsToRemove[i];
				Assert.Same("Parent of a void is always block", a.obj.GetType(), typeof(Block));
				
				a.obj.As<Block>().RemoveStatement(a.index);
				
			}

		}

		protected override void OnFinish() {

			SelectiveDebug.StopTimer("VerifyRules");
			SelectiveDebug.LogRuleExecutor("Finished VerifyRules Job");

        }

    }

}