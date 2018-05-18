using RuleEngine;
using System;

namespace RuleEngineAddons.TurnBased {

	public class MoveProcessorSynchronous {

		protected Engine E;
		protected Unit unit;
		protected Position position;
        protected bool applyEffects;
        
		protected int collisionsAttempted;

		protected Action CancelMove;
		protected Action ContinueTurnChange;

        private BoardManager BoardManager;
        private TurnManager TurnManager;
        private TurnBasedExecutionManager TurnBasedExecutionManager;

        public MoveProcessorSynchronous(Engine E, Unit unit, Position position, Action cancelMove, Action continueTurnChange, bool applyEffects = true) {
			this.E = E;
			this.unit = unit;
			this.position = position;
			this.CancelMove = cancelMove;
			this.ContinueTurnChange = continueTurnChange;
            this.applyEffects = applyEffects;
            BoardManager = E.GetManager<BoardManager>();
            TurnManager = E.GetManager<TurnManager>();
            TurnBasedExecutionManager = (TurnBasedExecutionManager) E.ExecutionManager;

            collisionsAttempted = 0;

		}

		public bool MakeMove() {
			
			// Validate the move request
			Rule validRule;
            bool permitted = TurnManager.IsMovePermitted(unit, position, out validRule);
            if (!permitted) {

                if (applyEffects) {
                    E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("RuleError", "Move not allowed, play again.");
                    E.EffectFactory.EnqueueNewEffect<IMoveUnitEffect>(unit, BoardManager.GetPosition_TS(unit.GetOffset_TS()));
                }
				
				CancelMove.Invoke();
				return false;
			}

            // Save the unit that is moving for use inside the abstraction engine
            E.VariableManager.SetVariable_TS("Moving unit", BoardManager.UnitRegistry.CreateObjectValue_TS(unit));

            // Launch the movement process with a branch to resolve collisions or move on to checking the goals
            BoardManager.ApplyGameModification_TS(new MoveUnitGameModification(E, unit, position.GetOffset_TS(), applyEffects));

			return true;
		}

		public void ResolveCollisionsIfAny() {
			
			CollisionProfile collision;
			
			// No collisions
			if (!BoardManager.DetectUnitCollisions_TS()) {
				SelectiveDebug.LogCollision("No collisions");
                 
				ExecuteCollisionRulesWithNoCollisions();
				
				return;
			} 
			
			// No rules for dealing with collisions
			if (E.RuleManager.RuleTypeToList[TurnBasedExecutionManager.Collision].Count == 0) {
				SelectiveDebug.LogCollision("No collision rules");
				
				// Return to the original position
				collision = BoardManager.PopNextUnitCollision_TS();
				Assert.Null("Next collision should be null if there was only one move", BoardManager.PopNextUnitCollision_TS());

                BoardManager.ApplyGameModification_TS(new MoveUnitGameModification(E, collision.IncidentUnit, collision.IncidentUnit.lastOffset_TS, applyEffects));

				CancelMove.Invoke();

				return;
			}
			
			ResolveCollisionsUsingRules();

		}

		protected virtual void ResolveCollisionsUsingRules() {
			
			bool CollisionResolved;
			bool MoveCancelled;

			while (true) {

				// Attempt to resolve the next collision
				ResolveACollision(BoardManager.PopNextUnitCollision_TS (), out CollisionResolved, out MoveCancelled);
                 
				if (MoveCancelled) {
					SelectiveDebug.LogCollision("Move cancelled.");
                    if (applyEffects)
                        E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("RuleError", "Move was cancelled by the collision rules, play again.");

                    // Reset turn collision resolution count
                    TurnManager.CurrentTurn.CollisionResolutions -= collisionsAttempted;

                    E.ModificationManager.UndoAllModifications_TS();

                    // Recalculate the possible actions of units
                    TurnBasedExecutionManager.ExecuteMovementRulesSynchronously();

					CancelMove.Invoke();
					
					return;
				}
				
				// Check to make sure that this particular collision has been resolved
				if (!CollisionResolved) {
					SelectiveDebug.LogCollision("Collision not resolved by the collision rules. Cancelling move...");
                    if (applyEffects)
                        E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("RuleError", "Collision rules could not resolve these collisions, play again.");

                    // Reset turn collision resolution count
                    TurnManager.CurrentTurn.CollisionResolutions -= collisionsAttempted;

                    E.ModificationManager.UndoAllModifications_TS();

                    // Recalculate the possible actions of units
                    TurnBasedExecutionManager.ExecuteMovementRulesSynchronously();

					CancelMove.Invoke();

					return; 
				} 

				if (!BoardManager.DetectUnitCollisions_TS()) {
					SelectiveDebug.LogCollision("All collisions resolved. Continuing with changing turn...");
					
					ContinueTurnChange.Invoke();

					return;
				}

				SelectiveDebug.LogCollision("Next collision...");

			}

		}
		
		protected virtual void ExecuteCollisionRulesWithNoCollisions() {
			
			bool CollisionResolved;
			bool MoveCancelled;
            TurnBasedExecutionManager.ExecuteCollisionRulesWithoutCollisionSynchronously(E.ModificationManager.PeekLastModification_TS(), out CollisionResolved, out MoveCancelled);
			
			if (MoveCancelled) {
				CancelMove.Invoke();
			} else { 
				ContinueTurnChange.Invoke(); // Found possible action via here
			}
			
		}
		
		private void ResolveACollision(CollisionProfile collision, out bool CollisionResolved, out bool MoveCancelled) {
			
			if (!StartResolvingCollision (collision, out CollisionResolved, out MoveCancelled))
				return;

            TurnBasedExecutionManager.ExecuteCollisionRulesSynchronously(collision, E.ModificationManager.PeekLastModification_TS(), out CollisionResolved, out MoveCancelled);
			
            E.VariableManager.SetVariable_TS("Target unit", null);
			
		}

		protected bool StartResolvingCollision(CollisionProfile collision, out bool CollisionResolved, out bool MoveCancelled) {
			
			// Check to see if the number of collisions resolved has exceeded the maximum
			if (TurnManager.CurrentTurn.CollisionResolutions++ >= Turn.MAX_COLLISIONS_PER_TURN) {

                // Revert all modifications made during this move
                E.ModificationManager.UndoAllModifications_TS();

                if (applyEffects)
                    E.EffectFactory.EnqueueNewEffect<IRuleExceptionEffect>("RuleError", "Maximum number of collateral collision resolutions exceeded. Move cancelled");

                CollisionResolved = false;
				MoveCancelled = true;

				return false;
			} else {
			
				// Remember how many collisions we've attempted to resolve
				collisionsAttempted++;

                // Declare the unit that is moving

                E.VariableManager.SetVariable_TS("Moving unit", BoardManager.UnitRegistry.CreateObjectValue_TS(collision.IncidentUnit));

                // And set the stationary unit - the unit in the collision that hasn't moved
                E.VariableManager.SetVariable_TS("Target unit", BoardManager.UnitRegistry.CreateObjectValue_TS(collision.StationaryUnit));

                // Should not be used
                CollisionResolved = false;
				MoveCancelled = false;

				return true;
			}

		}

	}

}

