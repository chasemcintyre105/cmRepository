using RuleEngine;
using System;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {
	
	public class NoRuleUsableExpression : Expression {

		public override Type GetReturnType() {
			return typeof(BooleanValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Attributes";
		}

		public NoRuleUsableExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public override void DefineArguments() { 
            // No arguments
		}
        
		public override string GetDescription() {
			return "Determines if there are no usable rules, and hence no possible actions for the current player.";
		}

        public override string ToString() {
            return "NoRuleUsable";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            Value returnValue = null;

            switch (RuleExecutor.RuleType.ID) {
            case TurnBasedExecutionManager.MOVEMENT:
                Assert.Never("NoRuleUsableExpression not allowed in the Movement rules");
                break;
            case TurnBasedExecutionManager.COLLISION:
                Assert.Never("NoRuleUsableExpression not allowed in the Collision rules");
                break;
            case TurnBasedExecutionManager.TURN:

                BoardManager BoardManager = E.GetManager<BoardManager>();

                // Movement rules
                // ==============

                // Block the game modification stack from undoing past the current modification
                Modification savedBlock = E.ModificationManager.PeekLastModification_TS();
                E.ModificationManager.SetUndoBlock_TS(savedBlock);

                // Movement rules

                MoveProcessorSynchronous moveCoordinator;
                bool FoundPossibleAction = false;
                
                List<Unit> unitsAsOfNow = BoardManager.UnitRegistry.GenerateNewListOfAllObjects_TS();
                List<PossibleAction> possibleActionsAsOfNow;
                Player currentPlayer = ((TurnRuleExecutor) RuleExecutor).CurrentPlayer;

                foreach (Unit unit in unitsAsOfNow) {

                    if (unit.player != currentPlayer)
                        continue;

                    possibleActionsAsOfNow = new List<PossibleAction>(unit.GetPossibleActions_TS());

                    foreach (PossibleAction action in possibleActionsAsOfNow) {

                        moveCoordinator = new MoveProcessorSynchronous(E, unit, action.FinalPosition, delegate () {

                            // Cancel move
                            // Nothing to do

                        }, delegate () {

                            // Continue turn change
                            FoundPossibleAction = true;

                        }, false);

                        Assert.True("Possible actions should always be possible", moveCoordinator.MakeMove());

                        moveCoordinator.ResolveCollisionsIfAny();

                        if (FoundPossibleAction)
                            break;

                    }

                    if (FoundPossibleAction)
                        break;

                }

                // Restore game to previous state ready for next rule set
                E.ModificationManager.UndoAllModifications_TS();
                
                // Restore game to previous state ready for next rule set
                E.ModificationManager.UndoAllModifications_TS();

                // All other rules
                // ===============
                // Iterate through all of them to see if any result in no errors

                // Always finish a rule set by undoing all the modifications they may have made:
                // E.ModificationManager.UndoAllModifications_TS();


                // Finally, clear the undo block
                E.ModificationManager.ClearUndoBlock_TS();
                Assert.Same("The game modification from the beginning of the NoUsableRule statement and from the end match", savedBlock, E.ModificationManager.PeekLastModification_TS());

                // Set the return value
                returnValue = new BooleanValue(E, !FoundPossibleAction);
                break;
            }

            return returnValue;
        }
    }

}