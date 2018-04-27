using RuleEngine;
using RuleEngineAddons.RulePanel;
using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class UnitAttachment : IUnitAttachment {
		
		private Unit unit;
        private Notification currentNotification;

        private PanelManager _PanelManager;
        private PanelManager PanelManager
        {
            get
            {
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
        private TurnController BoardController
        {
            get
            {
                if (_BoardController == null)
                    _BoardController = RuleEngineController.E.GetController<TurnController>();

                return _BoardController;
            }
        }

        public override void SetBoardObject(IBoardObject value) {
			Assert.Is<Unit>("BoardObject is of type Unit", value);

			unit = (Unit) value;
		}

        public override IBoardObject GetBoardObject() {
			return unit;
		}

        public override bool HasBoardObject() {
			return unit != null;
		}

        public override GameObject GetGameObject() {
			return gameObject;
		}

        public override void OnMouseDown() { 
			if (BoardController.GameAcceptingUserInput) {
                PanelManager.UnitDraggerDropper.StartDragging (unit, this, gameObject);
			}
		}

        public override void OnMouseUp() { 
			if (BoardController.GameAcceptingUserInput)
                PanelManager.UnitDraggerDropper.StopDragging();
		}

        public override void OnMouseEnter() {
			if (BoardController.GameAcceptingUserInput) {
				Assert.NotNull ("Unit", unit);

				Position position = RuleEngineController.E.GetManager<BoardManager>().GetPosition_TS(unit.GetOffset_TS());
				Assert.NotNull ("Unit's position", position);

                PanelManager.UnitDraggerDropper.MouseEnteredDroppableObject(position, this);
			}
            if (!RulePanelManager.MainRulePanel.IsOpen) {
                currentNotification = new Notification() {
                    Text = unit.ToString()
                };
                NotificationManager.ShowNotification_TS(currentNotification);
            }
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
		}

        public override void UnpossibleAction() {
		}

    }
}