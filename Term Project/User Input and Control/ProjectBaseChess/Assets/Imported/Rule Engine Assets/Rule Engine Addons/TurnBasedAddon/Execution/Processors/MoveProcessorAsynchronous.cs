using RuleEngine;
using System;

namespace RuleEngineAddons.TurnBased {

	public class MoveProcessorAsynchronous : MoveProcessorSynchronous {

        private BoardManager BoardManager;
        private TurnManager TurnManager;
        private TurnBasedExecutionManager TurnBasedExecutionManager;

        public MoveProcessorAsynchronous(Engine E, Unit unit, Position position, Action cancelMove, Action continueTurnChange) : base(E, unit, position, cancelMove, continueTurnChange) {
            BoardManager = E.GetManager<BoardManager>();
            TurnManager = E.GetManager<TurnManager>();
            TurnBasedExecutionManager = (TurnBasedExecutionManager) E.ExecutionManager;
        }

        protected override void ExecuteCollisionRulesWithNoCollisions() {
			
			TurnBasedExecutionManager.ExecuteCollisionRulesWithoutCollision(E.ModificationManager.PeekLastModification_TS(), (Action<bool, bool>) delegate (bool CollisionResolved_UNUSED, bool MoveCancelled) {
				if (MoveCancelled) {
					CancelMove.Invoke();
				} else {
					ContinueTurnChange.Invoke();
				}
			});
			
		}

        protected override void ResolveCollisionsUsingRules() {
			ResolveACollisionAsynchronous(BoardManager.PopNextUnitCollision_TS(), CollisionResolutionLoop);
		}
		
		private void ResolveACollisionAsynchronous(CollisionProfile collision, Action<bool, bool> callback) {

			bool CollisionResolved;
			bool MoveCancelled;
			if (!StartResolvingCollision (collision, out CollisionResolved, out MoveCancelled)) {
				callback.Invoke(CollisionResolved, MoveCancelled);
				return;
			}

            TurnBasedExecutionManager.ExecuteCollisionRules(collision, E.ModificationManager.PeekLastModification_TS(), delegate (bool _CollisionResolved, bool _MoveCancelled) {
				
                E.VariableManager.SetVariable_TS("Target unit", null);
				callback.Invoke(_CollisionResolved, _MoveCancelled);
				
			});
			
		}

        private void CollisionResolutionLoop(bool CollisionResolved, bool MoveCancelled) {

            if (MoveCancelled) {
                SelectiveDebug.LogCollision("Move cancelled.");
                E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("RuleError", "Move was cancelled by the collision rules, play again.");

                // Reset turn collision resolution count
                TurnManager.CurrentTurn.CollisionResolutions -= collisionsAttempted;

                E.ModificationManager.UndoAllModifications_TS();

                TurnBasedExecutionManager.ExecuteMovementRules((Action) delegate {
                    CancelMove.Invoke();
                });

                return;
            }

            // Check to make sure that this particular collision has been resolved
            if (!CollisionResolved) {
                SelectiveDebug.LogCollision("Collision not resolved by the collision rules. Cancelling move...");
                E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("RuleError", "Collision rules could not resolve these collisions, play again.");

                // Reset turn collision resolution count
                TurnManager.CurrentTurn.CollisionResolutions -= collisionsAttempted;

                E.ModificationManager.UndoAllModifications_TS();

                TurnBasedExecutionManager.ExecuteMovementRules((Action) delegate {
                    CancelMove.Invoke();
                });

                return;
            }

            if (BoardManager.DetectUnitCollisions_TS()) {
                SelectiveDebug.LogCollision("Next collision...");

                ResolveACollisionAsynchronous(BoardManager.PopNextUnitCollision_TS(), CollisionResolutionLoop);

            } else {
                SelectiveDebug.LogCollision("All collisions resolved. Continuing with changing turn...");

                ContinueTurnChange.Invoke();
            }

        }

    }

}

