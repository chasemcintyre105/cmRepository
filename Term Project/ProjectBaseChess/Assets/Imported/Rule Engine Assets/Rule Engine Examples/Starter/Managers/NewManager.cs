using RuleEngine;
using UnityEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewManager : IManager {

        public override void Init() {
        }

        public override void Preinit() {
        }

        // Note that it is good practice to make all methods of a manager virtual so that they may be overriden by any future addon
        // To this end, no method, field or property should ever be private either
        public virtual void EnqueueSomeEffects() {
            E.EffectFactory.EnqueueNewEffect<INewEffect>(0, "Second parameter");
            E.EffectFactory.EnqueueNewEffect<INewEffect>(0, "Second parameter", "Third optional parameter");
            E.EffectFactory.EnqueueNewEffect<NewDisplayMessageEffect>("A message to display");
        }

        public virtual void ExecuteNewRuleSynchronously(string uid) {
            Debug.Log("Executing new rule synchronously: " + uid);

            Rule rule = null;
            Assert.True("Rule exists with uid: " + uid, E.RuleManager.RulesByUID.TryGetValue(uid, out rule));
            new NewRuleExecutor(E).ExecuteRule(rule);
        }

        public virtual void ExecuteNewRulesSynchronously() {
            Debug.Log("Executing new rules synchronously");

            new NewRuleExecutor(E).ExecuteRules();
        }

        public virtual void ExecuteNewRuleAsynchronously(string ruleID) {
            NewRulesJob job = new NewRulesJob();
            job.E = E;
            job.RuleUID = ruleID;
            job.InitialiseRuleExecutor();

            E.ThreadController.ExecuteThreadedJob(job);
        }

        public virtual void ExecuteNewRulesAsynchronously() {
            NewRulesJob job = new NewRulesJob();
            job.E = E;
            job.InitialiseRuleExecutor();

            E.ThreadController.ExecuteThreadedJob(job);
        }

    }

}