using RuleEngine;
using System;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

	public class TurnTransitionHandler : TransitionHandler<TurnEvent, TurnState> {

		private MoveProcessorAsynchronous moveCoordinator;

        protected TurnManager TurnManager;

        public TurnTransitionHandler(Engine E) : base(E) {
        }

        public override void Init() {
            TurnManager = E.GetManager<TurnManager>();
        }

        public void Handle_Wait_For_Input(Dictionary<string, object> properties) {
			// Nothing to do, just chillin'
		}

		public void Handle_Make_Move(Dictionary<string, object> properties) {
			Unit unit = GetProperty<Unit>(properties, "Unit");
			Position position = GetProperty<Position>(properties, "Position");

			Assert.True("The unit belongs to the current player", unit.player == TurnManager.CurrentTurn.player);

			moveCoordinator = new MoveProcessorAsynchronous(E, unit, position, delegate() {

                // Cancel move
                TurnManager.TurnStateMachine.IssueCommand (TurnEvent.Wait_For_Input);

			}, delegate() {

                // Continue turn change
                TurnManager.TurnStateMachine.IssueCommand (TurnEvent.Switch_Players);

			});

			// Make move, and if it resolves, move on to collisions
			if (moveCoordinator.MakeMove())
                TurnManager.TurnStateMachine.IssueCommand(TurnEvent.Resolve_Collisions);

		}

		public void Handle_Resolve_Collisions(Dictionary<string, object> properties) {
			moveCoordinator.ResolveCollisionsIfAny();
		}
		 
		public void Handle_Switch_Players(Dictionary<string, object> properties) {

			TurnManager.RequestNextTurn((Action<bool>) delegate (bool MoveCancelled) {
                TurnManager.TurnStateMachine.IssueCommand(TurnEvent.Wait_For_Input);
			});

		}

        public void Handle_Place_Object(Dictionary<string, object> properties) {
            TileObjectTypeValue TileObjectType = GetProperty<TileObjectTypeValue>(properties, "ObjectType");

            E.EffectFactory.EnqueueNewEffect<IStartGameObjectPlacementEffect>(TileObjectType.TileType);

        }

    }

}


