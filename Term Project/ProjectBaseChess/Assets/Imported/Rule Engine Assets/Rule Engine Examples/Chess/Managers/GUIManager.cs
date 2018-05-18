using RuleEngine;
using RuleEngineAddons.RulePanel;
using RuleEngineAddons.TurnBased;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class GUIManager : IManager {

        public BoardEventable UnitDraggerDropper { get; private set; }
        public WinnerDialogAttachment WinnerDialog; // Self registered, see WinnerDialogAttachment

        protected BoardManager BoardManager;
        protected List<Position> PossibledPositions;

        public override void Preinit() {
            PossibledPositions = new List<Position>();
        }

        public override void Init() {
            BoardManager = E.GetManager<BoardManager>();
        }

        public virtual void StartGameObjectPlacement(IObjectType type) {

            IBoardObject placingObject = null;

            if (type is TileType) {

                // Create an instance of the tile type
                new TileCreator_TS(E, type as TileType, Vector3.zero)
                    .SetAsPlacemarker()
                    .Finalise(out placingObject);

                E.EffectFactory.EnqueueNewEffect<IRemoveTileEffect>((Tile) placingObject);

            } else
                Assert.Never("Game object type not supported: " + type.GetType().Name);

            // Show possible placements
            // And give each a script that when hovered by the mouse, the tile is placed there but with a different material and on a different layer it doesn't interfer
            BoardManager.ShowPossibleTilePlacements(placingObject);

        }

        public virtual void StopGameObjectPlacement() {

            // Hide possible placements
            BoardManager.HidePossibleTilePlacements();

            // Clear the last
            Assert.True("The last modification was a RemoveStackObjectModification", E.ModificationManager.PeekLastModification_TS() is RemoveStackObjectModification);
            E.ModificationManager.UndoModificationsUpToAndIncluding_TS(E.ModificationManager.PeekLastModification_TS());

        }

        public virtual void ShowPossibleActionsForUnit(Unit unit) { // Possible extension: show possible actions for multiple unit, allow multiple selection of units
            foreach (PossibleAction action in unit.GetPossibleActions_TS()) {
                ((PositionAttachment) action.FinalPosition.GetBoardObjectAttachment()).PossibleAction();
                PossibledPositions.Add(action.FinalPosition);
            }
        }

        public virtual void HidePossibleActions() {
            foreach (Position possibledPosition in PossibledPositions) {
                ((PositionAttachment) possibledPosition.GetBoardObjectAttachment()).UnpossibleAction();
            }
            PossibledPositions.Clear();
        }

    }

}

