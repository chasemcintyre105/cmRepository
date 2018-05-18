using System;
using RuleEngine;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public class TurnManager : IManager {
        
        public delegate void TurnChangeAction();
		public event TurnChangeAction OnTurnChanged;

        public delegate void BeforeTurnChangeAction();
        public event BeforeTurnChangeAction OnBeforeTurnChange;

        public delegate void TurnCancelledAction();
        public event TurnCancelledAction OnTurnCancelled;

        public Turn CurrentTurn { get; private set; }
        public Turn PreviousTurn { get; private set; }

        // State machine
        public StateMachine<TurnEvent, TurnState> TurnStateMachine;
        
        protected BoardManager BoardManager;
        protected PlayerManager PlayerManager;
        
        public override void Preinit() {
			CurrentTurn = new Turn();
        }

        public override void Init() {
        }

        public virtual void SetupFirstTurn() {

            BoardManager = E.GetManager<BoardManager>();
            PlayerManager = E.GetManager<PlayerManager>();

            // Initialise turn object
            CurrentTurn.number = 1;
            CurrentTurn.player = PlayerManager.FirstPlayer;

            // Set visibility of player objects
            foreach (Player player in PlayerManager.Players) {
                bool isCurrentPlayer = player == PlayerManager.FirstPlayer;
                player.TurnDisplay.SetActive(isCurrentPlayer);
                player.CoordinateOrigin.SetActive(isCurrentPlayer);
            }

            E.EffectFactory.EnqueueNewEffect<ITurnChangeEffect>(CurrentTurn.player);

        }

        public virtual void RequestNextTurn(Action<bool> callback) {

            // Unset all relevant variables
            E.VariableManager.SetVariable_TS("Moving unit", null);
			Assert.True("Target unit is not set", E.VariableManager.GetVariable_TS("Target unit").VariableValue.GetType() == typeof(NullValue));

            // Update possible actions and run the turn rules
			((TurnBasedExecutionManager) E.ExecutionManager).ExecuteMovementRules((Action) delegate {
                
                /* Do turn rules at the end of turn
                E.ExecutionManager.ExecuteTurnRules((Action<bool>) delegate (bool MoveCancelled) {

                    if (MoveCancelled) {
                        callback.Invoke(true);
                    } else {
                        SetNextTurn(callback);
                    }
                    
				});*/

                // Do turn rules at the beginning of turn
                SetNextTurn(callback);

			});
			
		}
		
		protected virtual void SetNextTurn(Action<bool> callback) {

            PreviousTurn = CurrentTurn;
			CurrentTurn = new Turn();
			
			// Select next player
			CurrentTurn.player = PlayerManager.Players[(PreviousTurn.player.UID + 1) % PlayerManager.PlayersByPosition.Count];

			if (PlayerManager.PlayersByPosition.Count > 1)
				Assert.True("Player changes from turn to turn", PreviousTurn.player != CurrentTurn.player);

            if (OnBeforeTurnChange != null)
                OnBeforeTurnChange();

            ((TurnBasedExecutionManager) E.ExecutionManager).ExecuteTurnRules((Action<bool>) delegate (bool MoveCancelled) {

                if (MoveCancelled) {

                    CurrentTurn = PreviousTurn;

                    if (OnTurnCancelled != null)
                        OnTurnCancelled();

                    callback.Invoke(MoveCancelled);

                } else {

                    // Final refresh of movement rules before player is given control
                    ((TurnBasedExecutionManager) E.ExecutionManager).ExecuteMovementRules(delegate {

                        // Clear the judgement of the RuleJudge
                        if (E.RuleManager.RuleJudge != null)
                            E.RuleManager.RuleJudge.ClearJudgement();

                        // Clear all rule and board modifications
                        E.RuleManager.ModStack.ClearModifications();
                        E.ModificationManager.ClearModifications_TS();

                        // Switch player displays
                        PreviousTurn.player.TurnDisplay.SetActive(false);
                        CurrentTurn.player.TurnDisplay.SetActive(true);

                        // Switch player coordinate origin
                        PreviousTurn.player.CoordinateOrigin.SetActive(false);
                        CurrentTurn.player.CoordinateOrigin.SetActive(true);

                        if (OnTurnChanged != null)
                            OnTurnChanged();

                        SelectiveDebug.LogRuleSet("Turn changed");

                        E.EffectFactory.EnqueueNewEffect<ITurnChangeEffect>(CurrentTurn.player);

                        callback.Invoke(MoveCancelled);

                    });

                }

            });
        }

        public virtual bool IsMovePermitted(Unit unit, Position position) {
            if (unit.player == CurrentTurn.player) {
                foreach (PossibleAction action in unit.GetPossibleActions_TS()) {
                    if (action.FinalPosition == position) {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual bool IsMovePermitted(Unit unit, Position position, out Rule validRule) {
            foreach (PossibleAction action in unit.GetPossibleActions_TS()) {
                if (action.FinalPosition == position) {
                    validRule = action.rule;
                    return true;
                }
            }
            validRule = null;
            return false;
        }

    }

}