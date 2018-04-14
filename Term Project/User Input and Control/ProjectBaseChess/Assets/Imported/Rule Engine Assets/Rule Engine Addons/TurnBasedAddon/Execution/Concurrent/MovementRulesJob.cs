using RuleEngine;
using System;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

	public class MovementRulesJob : ThreadedJob {

		public Engine E;
		public MovementRuleExecutor RuleExecutor;
		public Player currentPlayer;
		public Modification BeginningMod;

        // NoRuleUsable
        public bool NoRuleUsableEnabled = false;
		public Action<Rule> BeforeRule;
		public Action<Rule> AfterRule;

		public MovementRulesJob() : base ("MovementRulesJob") { }

        private BoardManager BoardManager;
        private TurnBasedExecutionManager TurnBasedExecutionManager;

        public void InitialiseRuleExecutor() {

            BoardManager = E.GetManager<BoardManager>();
            TurnBasedExecutionManager = (TurnBasedExecutionManager) E.ExecutionManager;

            RuleExecutor = new MovementRuleExecutor(E, currentPlayer) {
				AfterFinishRule = (Action<Rule>) delegate (Rule rule) {

					if (!RuleExecutor.ErrorWasGenerated()) {

						// Register the executed move commands as possible actions for the board units
						foreach (UnitObjectValue unitObj in RuleExecutor.PossibleMovementsOfUnitVariables.Keys) {
							Unit unit = unitObj.GetInstance();
							Vector3 originalPosition = unit.GetOffset_TS();
							foreach (Vector3 possibleMove in RuleExecutor.PossibleMovementsOfUnitVariables[unitObj]) {
								Position possiblePosition = BoardManager.GetPosition_TS(possibleMove);
								if (originalPosition != possibleMove && possiblePosition != null) {
									unit.AddPossibleAction_TS(new PossibleAction(rule, possiblePosition));
								}
							}
						}

					} else if (RuleExecutor.ErrorWasImportant()) {
                        E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("Error", "Movement rule " + E.RuleManager.RuleTypeToList[TurnBasedExecutionManager.Movement].IndexOf(rule) + " returned the error: " + RuleExecutor.GetErrorMessage());
                    }

                    // Revert board state to that just before this rule was executed
                    E.ModificationManager.UndoModificationsUpTo_TS(BeginningMod);
					
				}
			};

		}

		public void InitialiseRuleExecutorForNoRuleUsable() {

			RuleExecutor = new MovementRuleExecutor(E, currentPlayer) {
				BeforeStartRule = BeforeRule,
				AfterFinishRule = AfterRule
			};

		}

		protected override void OnStart() {

			Assert.NotNull("RuleExecutor not null", RuleExecutor);

			SelectiveDebug.LogRuleExecutor("Starting ExecuteMovementRules");
			SelectiveDebug.StartTimer("MovementRules");
			
			BeginningMod = E.ModificationManager.PeekLastModification_TS();

            // Clear all possible actions, ready to be refilled in the RuleExecutor
            if (!NoRuleUsableEnabled)
                foreach (Unit unit in BoardManager.UnitRegistry.AllObjectsEnumerable_TS())
					unit.ClearPossibleActions_TS();

            // Run the RuleExecutor for each combination of variables declared in each rule
            RuleExecutor.RuleVariableIterator = "ForEachObject";
            RuleExecutor.ExecuteRules();

			SelectiveDebug.LogRuleExecutor("Finished ExecuteMovementRules");
		}

		protected override void OnFinish() {

			SelectiveDebug.StopTimer("MovementRules");

		}

	}

}

