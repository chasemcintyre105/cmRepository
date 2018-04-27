using RuleEngine;
using UnityEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewRulesJob : ThreadedJob {

		public Engine E;
		public NewRuleExecutor RuleExecutor;
        public string RuleUID = null;

        public NewRulesJob() : base ("NewRulesJob") { }

        public void InitialiseRuleExecutor() {
			RuleExecutor = new NewRuleExecutor(E);
		}

		protected override void OnStart() {
            // Run in a new thread

            if (RuleUID == null) { 
                Debug.Log("Executing new rules asynchronously");

                RuleExecutor.ExecuteRules();
            } else {
                Debug.Log("Executing new rule asynchronously: " + RuleUID);

                Rule rule = null;
                Assert.True("Rule exists with uid: " + RuleUID, E.RuleManager.RulesByUID.TryGetValue(RuleUID, out rule));
                new NewRuleExecutor(E).ExecuteRule(rule);
            }

		}

		protected override void OnFinish() {
            // Run in the main thread

            Debug.Log("Thread finished");

		}

	}

}

