using RuleEngine;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public class TurnRulesJob : ThreadedJob {

		public Engine E;
		public TurnRuleExecutor RuleExecutor;
		public Player currentPlayer;
		public bool MoveCancelled;

        public TurnRulesJob() : base ("TurnRulesJob") { }

        private RuleType turnType;

        public void InitialiseRuleExecutor() {

            turnType = ((TurnBasedExecutionManager) E.ExecutionManager).Turn;

			RuleExecutor = new TurnRuleExecutor(E, currentPlayer) {
				AfterFinishRule = delegate (Rule rule) {

					if (RuleExecutor.MoveCancelled) {

                        // Stop the RuleExecutor, undo all changes to the board since the beginning of the turn and rerun the movement rules
                        RuleExecutor.StopExecutingRules = true;
                        E.ModificationManager.UndoAllModifications_TS();
						((TurnBasedExecutionManager) E.ExecutionManager).ExecuteMovementRulesSynchronously();

                        if (rule.Name != null)
                            E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("RuleError", "Move was cancelled because of turn rule " + rule.Name + ", retry.");
                        else
                            E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("RuleError", "Move was cancelled because of turn rule " + (E.RuleManager.RuleTypeToList[turnType].IndexOf(rule) + 1) + ", retry.");

                    } else if (!RuleExecutor.ErrorWasGenerated()) {

					} else if (RuleExecutor.ErrorWasImportant()) {
                        E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("Error", "Turn rule " + E.RuleManager.RuleTypeToList[turnType].IndexOf(rule) + " returned the error: " + RuleExecutor.GetErrorMessage());
                    }

                }
			};

		}

		protected override void OnStart() {

			Assert.NotNull("RuleExecutor not null", RuleExecutor);

			SelectiveDebug.LogRuleExecutor("Starting ExecuteTurnRules");
			SelectiveDebug.StartTimer("TurnRules");

            // Run the RuleExecutor for each combination of variables declared in each rule
            RuleExecutor.RuleVariableIterator = "ForEachObject";
            RuleExecutor.ExecuteRules();

            // If there are more than one, declare them as winners
            if (RuleExecutor.playersDeclaredWinners.Count > 0)
                E.EffectFactory.EnqueueNewEffect<IAnnouceWinnersEffect>(RuleExecutor.playersDeclaredWinners, E.GetManager<PlayerManager>().IsMultiplayer(), null);
			
			// If there are more than one, declare them as winners
			if (RuleExecutor.playersDeclaredLosers.Count > 0)
                E.EffectFactory.EnqueueNewEffect<IAnnouceLosersEffect>(RuleExecutor.playersDeclaredLosers, E.GetManager<PlayerManager>().IsMultiplayer());

			SelectiveDebug.LogRuleExecutor("Finished ExecuteTurnRules");
		}

		protected override void OnFinish() {

			SelectiveDebug.StopTimer("TurnRules");
			
			// Set output
			MoveCancelled = RuleExecutor.MoveCancelled;

		}

	}

}

