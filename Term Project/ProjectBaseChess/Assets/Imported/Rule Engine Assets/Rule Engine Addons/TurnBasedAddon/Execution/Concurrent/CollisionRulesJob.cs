using RuleEngine;
using System;

namespace RuleEngineAddons.TurnBased {

	public class CollisionRulesJob : ThreadedJob {

		public Engine E;
		public CollisionRuleExecutor RuleExecutor;
		public CollisionProfile collision;
		public Modification BeginningOfLastRuleMod;
		public bool CollisionResolved;
		public bool MoveCancelled;

        private TurnManager TurnManager;
        private TurnBasedExecutionManager TurnBasedExecutionManager;

        public CollisionRulesJob() : base ("CollisionRulesJob") {
        }

		public void InitialiseRuleExecutor() {

            TurnManager = E.GetManager<TurnManager>();
            TurnBasedExecutionManager = (TurnBasedExecutionManager) E.ExecutionManager;

            // Set up the collision rule executor
            RuleExecutor = new CollisionRuleExecutor(E, TurnManager.CurrentTurn.player) {
				BeforeStartRule = (Action<Rule>) delegate (Rule rule) {

					// Skip rule if it is marked as collision only and there was no collision passed
					if (rule.CollisionOnly && collision == null) {

                        SelectiveDebug.LogExecution("Marking rule for skipping since it is collision only");
						RuleExecutor.SkipRule = true;
						
					} else if (rule.PossibleActionDependent) {
						
						// The possible actions for each unit need to be recalculated since there are functions that depend on the possible actions of the units
						((TurnBasedExecutionManager) E.ExecutionManager).ExecuteMovementRulesSynchronously();
						
					}
					
				},
				AfterFinishRule = (Action<Rule>) delegate (Rule rule) {

					if (RuleExecutor.MoveCancelled) {
                        SelectiveDebug.LogRuleSet("CollisionRulesJob: MoveCancelled");

                        // Stop the RuleExecutor, undo all changes to the board and rerun the movement rules in case any units had moved
                        RuleExecutor.StopExecutingRules = true;
                        E.ModificationManager.UndoAllModifications_TS();
                        ((TurnBasedExecutionManager) E.ExecutionManager).ExecuteMovementRulesSynchronously();
                        
						SelectiveDebug.LogCollision("Move was cancelled because of collision rule " + (E.RuleManager.RuleTypeToList[TurnBasedExecutionManager.Collision].IndexOf(rule) + 1) + ", retry.");

					} else if (RuleExecutor.ErrorWasGenerated()) {
						
                        E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("Error", "Collision rule " + (E.RuleManager.RuleTypeToList[TurnBasedExecutionManager.Collision].IndexOf(rule) + 1).ToString() + " returned the error: " + RuleExecutor.GetErrorMessage());

                        // Undo the modifications made up until the beginning of the mods for this collision
                        E.ModificationManager.UndoModificationsUpTo_TS(BeginningOfLastRuleMod); 
						
					} else {
						 
						if (collision != null) {
							CollisionResolved = collision.IsCollisionResolved();
							if (CollisionResolved) {
								TurnManager.CurrentTurn.CollisionResolutions++;
                                RuleExecutor.StopExecutingRules = true;
							}
						}

						if (RuleExecutor.LastModificationMade != null)
							BeginningOfLastRuleMod = RuleExecutor.LastModificationMade;
						
					}
					
				}
			};

		}

		protected override void OnStart() {

			SelectiveDebug.LogRuleExecutor("Started ExecuteCollisionRules" + ((collision == null) ? "WithoutCollision" : "" ));
			SelectiveDebug.StartTimer("CollisionRules");

			RuleExecutor.MoveCancelled = false;
            RuleExecutor.RuleVariableIterator = "ForEachObject";
            CollisionResolved = false;

			RuleExecutor.ExecuteRules();

			// If there are more than one, declare them as winners
			if (RuleExecutor.playersDeclaredWinners.Count > 0)
                E.EffectFactory.EnqueueNewEffect<IAnnouceWinnersEffect>(RuleExecutor.playersDeclaredLosers, E.GetManager<PlayerManager>().IsMultiplayer(), null);

            // If there are more than one, declare them as winners
            if (RuleExecutor.playersDeclaredLosers.Count > 0)
                E.EffectFactory.EnqueueNewEffect<IAnnouceLosersEffect>(RuleExecutor.playersDeclaredLosers, E.GetManager<PlayerManager>().IsMultiplayer());

			SelectiveDebug.LogRuleExecutor("Finished ExecuteCollisionRules" + ((collision == null) ? "WithoutCollision" : "" ));

		}

		protected override void OnFinish() {

			SelectiveDebug.StopTimer("CollisionRules");

			// Set output
			MoveCancelled = RuleExecutor.MoveCancelled;

		}

	}

}

