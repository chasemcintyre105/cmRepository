using RuleEngine;
using RuleEngineAddons.RulePanel;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public class BoardEventable : DragAndDropEventable<Unit, IBoardObject> {

        protected Engine E;

        protected TurnManager TurnManager;
        protected BoardManager BoardManager;
        protected TurnController BoardController;
        protected RulePanelManager RulePanelManager;

        public BoardEventable(Engine E) {
            this.E = E;
            TurnManager = E.GetManager<TurnManager>();
            BoardManager = E.GetManager<BoardManager>();
            RulePanelManager = E.GetManager<RulePanelManager>();
            BoardController = E.GetController<TurnController>();
            AlwaysDrop = true; // Allows the returning animation when the drop is not available
		}

        protected override bool CanDrag(Unit unit) {
            if (RulePanelManager == null) {
                return unit.player == TurnManager.CurrentTurn.player &&
                       TurnManager.TurnStateMachine.CurrentState == TurnState.Waiting;
            } else {
                return !RulePanelManager.MainRulePanel.IsOpen &&
                       unit.player == TurnManager.CurrentTurn.player &&
                       TurnManager.TurnStateMachine.CurrentState == TurnState.Waiting;
            }
        }

        protected override void StartedDragging() {
            E.EffectFactory.EnqueueNewEffect<IShowPossibleActionsForUnitEffect>(draggable);
		}

		protected override void StoppedDragging() {
            E.EffectFactory.EnqueueNewEffect<IHidePossibleActionsEffect>();
        }

        protected override void Dropped() {

			if (IsMouseOverBoard()) {
				SelectiveDebug.LogDragDrop("Dropped with BoardEventable: " + draggable.type.ToString() + " -> " + droppable.GetOffset_TS().ToString());

				Dictionary<string, object> properties = new Dictionary<string, object>();
				properties.Add("Unit", draggable);
				properties.Add("Position", droppable);

                TurnManager.TurnStateMachine.IssueCommand(TurnEvent.Make_Move, properties);

			}

		}

        public bool IsMouseOverBoard() {
            Ray vRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;
            if (Physics.Raycast(vRay, out hitInfo)) {

                // Check whether the mouse is over a unit
                if (hitInfo.collider.gameObject.GetComponent<IPositionAttachment>() != null) {
                    SelectiveDebug.LogDragDropPermitted("Found position: " + hitInfo.collider.gameObject.name);
                    return true;
                }

                // Check whether the mouse is over a unit
                if (hitInfo.collider.gameObject.GetComponent<IUnitAttachment>() != null) {
                    SelectiveDebug.LogDragDropPermitted("Found unit: " + hitInfo.collider.gameObject.name);
                    return true;
                }

                // Check whether the mouse is over a tile
                if (hitInfo.collider.gameObject.GetComponent<ITileAttachment>() != null) {
                    SelectiveDebug.LogDragDropPermitted("Found tile: " + hitInfo.collider.gameObject.name);
                    return true;
                }

                // Check whether the mouse is over a mesh object
                IMeshAttachment mesh = hitInfo.collider.gameObject.GetComponent<IMeshAttachment>();
                if (mesh != null) {
                    SelectiveDebug.LogDragDropPermitted("Found mesh with attached " + mesh.GetAttachment().GetType().Name);
                    return true;
                }

                SelectiveDebug.LogDragDropPermitted("Hit unknown: " + hitInfo.collider.gameObject.name);

                return false;

            }

            SelectiveDebug.LogDragDropPermitted("Found nothing");

            return false;

        }

        protected override bool IsDropPermitted() {

			if (droppable == null) {
				SelectiveDebug.LogDragDropPermitted("Was null");
				return false;
			}

			if (!IsMouseOverBoard()) {
                SelectiveDebug.LogDragDropPermitted("Mouse not over board");
				return false;
			}
            
            if (droppable is Position) {
                SelectiveDebug.LogDragDropPermitted("Dropping onto position");
                return TurnManager.IsMovePermitted(draggable, droppable as Position);
            } else if (droppable is Unit) {
                SelectiveDebug.LogDragDropPermitted("Dropping onto unit");
                Vector2 offset = (droppable as Unit).GetOffset_TS();
                Position position = BoardManager.GetPosition_TS(offset);
                Assert.NotNull("There is a position for the unit at " + offset, position);
                return TurnManager.IsMovePermitted(draggable, position);
            } else {
                SelectiveDebug.LogDragDropPermitted("Dropping onto unknown and not allowed");
                return false;
            }

        }

    }

}

