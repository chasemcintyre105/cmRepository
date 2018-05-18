using RuleEngine;
using RuleEngineAddons.RulePanel;
using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class PositionAttachment : IPositionAttachment {

        private Position position;
        private Notification currentNotification;

        private PanelManager _PanelManager;
        private PanelManager PanelManager {
            get {
                if (_PanelManager == null)
                    _PanelManager = RuleEngineController.E.GetManager<PanelManager>();

                return _PanelManager;
            }
        }

        private RulePanelManager _RulePanelManager;
        private RulePanelManager RulePanelManager
        {
            get
            {
                if (_RulePanelManager == null)
                    _RulePanelManager = RuleEngineController.E.GetManager<RulePanelManager>();

                return _RulePanelManager;
            }
        }

        private NotificationManager _NotificationManager;
        private NotificationManager NotificationManager
        {
            get
            {
                if (_NotificationManager == null)
                    _NotificationManager = RuleEngineController.E.GetManager<NotificationManager>();

                return _NotificationManager;
            }
        }

        private TurnController _BoardController;
        private TurnController BoardController {
            get {
                if (_BoardController == null)
                    _BoardController = RuleEngineController.E.GetController<TurnController>();

                return _BoardController;
            }
        }

        private ChessSettingsController _CommonSettingsController;
        private ChessSettingsController CommonSettingsController
        {
            get
            {
                if (_CommonSettingsController == null)
                    _CommonSettingsController = RuleEngineController.E.GetController<ChessSettingsController>();

                return _CommonSettingsController;
            }
        }

        public override void SetBoardObject(IBoardObject value) {
            Assert.Is<Position>("BoardObject is of type Position", value);

            position = (Position) value;
        }

        public override IBoardObject GetBoardObject() {
            Assert.NotNull("Position", position);

            return position;
        }

        public override bool HasBoardObject() {
            return position != null;
        }

        public override GameObject GetGameObject() {
            return gameObject;
        }

        public override void OnMouseDown() { }
        public override void OnMouseUp() { }

        public override void OnMouseEnter() {
            Assert.NotNull("position", position);
            if (BoardController.GameAcceptingUserInput && gameObject.layer != 2) {
                PanelManager.UnitDraggerDropper.MouseEnteredDroppableObject(position, this);
            }
            if (!RulePanelManager.MainRulePanel.IsOpen) {
                Tile tile;
                string tooltip = "";
                if (position.TryGetAdjacentTileAtSameOffset(out tile)) {
                    tooltip = tile.ToString();
                } else {
                    tooltip = position.ToString();
                }
                currentNotification = new Notification() {
                    Text = tooltip
                };
                NotificationManager.ShowNotification_TS(currentNotification);
            }
        }
        
        public void OnMouseOver() {
            
        }

        public void OnMouseExit() {
            if (!RulePanelManager.MainRulePanel.IsOpen && currentNotification != null) { 
                NotificationManager.HideNotification_TS(currentNotification);
                currentNotification = null;
            }
        }

        public override void Hover() {

        }

        public override void Unhover() {

        }

        public override void PossibleAction() {
            if (gameObject.layer != 2) {
                gameObject.GetComponent<Renderer>().material = CommonSettingsController.BoardTemplates.PossibleActionMaterial;
            }
        }

        public override void UnpossibleAction() {
            if (gameObject.layer != 2) {
                gameObject.GetComponent<Renderer>().material = CommonSettingsController.BoardTemplates.UnpossibleActionMaterial;
            }
        }
        
    }
}