using RuleEngine;
using System;

namespace RuleEngineAddons.TurnBased {

    public class TurnBasedExecutionManager : ExecutionManager {

        public const string MOVEMENT = "Movement";
        public const string COLLISION = "Collision";
        public const string TURN = "Turn";

        public RuleType Movement;
        public RuleType Collision;
        public RuleType Turn;

        public virtual void ExecuteMovementRulesForNoRuleUsable(Action<MovementRuleExecutor> ReturnRuleExecutor, Action<Rule> BeforeRule, Action<Rule> AfterRule) {
            SelectiveDebug.LogRuleSet("MovementRules Sync (NoRuleUsable)");

            MovementRulesJob job = new MovementRulesJob();
			job.E = E;
			job.currentPlayer = E.GetManager<TurnManager>().CurrentTurn.player;
			job.NoRuleUsableEnabled = true;
			job.BeforeRule = BeforeRule;
			job.AfterRule = AfterRule;

			job.InitialiseRuleExecutorForNoRuleUsable();
			ReturnRuleExecutor.Invoke(job.RuleExecutor);

			job.RunJobInCurrentThread();
			
		}

        public virtual void ExecuteMovementRulesSynchronously() {
            SelectiveDebug.LogRuleSet("MovementRules Sync");

            MovementRulesJob job = new MovementRulesJob();
			job.E = E;
			job.currentPlayer = E.GetManager<TurnManager>().CurrentTurn.player;
			job.InitialiseRuleExecutor();

			job.RunJobInCurrentThread();

		}

        public virtual void ExecuteMovementRules(Action callback = null) {
            SelectiveDebug.LogRuleSet("MovementRules Async");

            MovementRulesJob job = new MovementRulesJob();
			
			job.E = E;
			job.currentPlayer = E.GetManager<TurnManager>().CurrentTurn.player;
			job.InitialiseRuleExecutor();

			E.ThreadController.ExecuteThreadedJob(job, callback);

		}

        public virtual void ExecuteTurnRules(Action<bool> callback = null) {
            SelectiveDebug.LogRuleSet("TurnRules Async");

            TurnRulesJob job = new TurnRulesJob();
			
			job.E = E;
			job.currentPlayer = E.GetManager<TurnManager>().CurrentTurn.player;
			job.InitialiseRuleExecutor();
			
			E.ThreadController.ExecuteThreadedJob(job, (Action) delegate {
				callback.Invoke(job.MoveCancelled);
			});

		}

        public virtual void ExecuteTurnRulesSynchronously(out bool MoveCancelled) {
            SelectiveDebug.LogRuleSet("TurnRules Sync");

            TurnRulesJob job = new TurnRulesJob();

            job.E = E;
            job.currentPlayer = E.GetManager<TurnManager>().CurrentTurn.player;
            job.InitialiseRuleExecutor();

            job.RunJobInCurrentThread();

            MoveCancelled = job.MoveCancelled;

        }

        public virtual void ExecuteCollisionRulesSynchronously(CollisionProfile collision, Modification BeginningOfLastRuleMod, out bool CollisionResolved, out bool MoveCancelled) {
            SelectiveDebug.LogRuleSet("CollisionRules Sync" + ((collision == null) ? " No Collision" : ""));

            CollisionRulesJob job = new CollisionRulesJob();
			job.E = E;
			job.collision = collision;
			job.BeginningOfLastRuleMod = BeginningOfLastRuleMod;
			job.InitialiseRuleExecutor();
			
			job.RunJobInCurrentThread();

			CollisionResolved = job.CollisionResolved;
			MoveCancelled = job.MoveCancelled;

		}

        public virtual void ExecuteCollisionRules(CollisionProfile collision, Modification BeginningOfLastRuleMod, Action<bool, bool> callback) {
            SelectiveDebug.LogRuleSet("CollisionRules Async");

            CollisionRulesJob job = new CollisionRulesJob();
			job.E = E;
			job.collision = collision;
			job.BeginningOfLastRuleMod = BeginningOfLastRuleMod;
			job.InitialiseRuleExecutor();

			E.ThreadController.ExecuteThreadedJob(job, (Action) delegate {
				callback.Invoke(job.CollisionResolved, job.MoveCancelled);
			});

		}

        public virtual void ExecuteCollisionRulesWithoutCollision(Modification BeginningOfLastRuleMod, Action<bool, bool> callback) {
			ExecuteCollisionRules(null, BeginningOfLastRuleMod, callback);
		}

        public virtual void ExecuteCollisionRulesWithoutCollisionSynchronously(Modification BeginningOfLastRuleMod, out bool CollisionResolved, out bool MoveCancelled) {
			ExecuteCollisionRulesSynchronously(null, BeginningOfLastRuleMod, out CollisionResolved, out MoveCancelled);
		}

    }

}

