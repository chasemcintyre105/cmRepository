using RuleEngine;
using RuleEngineAddons.RulePanel;
using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class PlacemarkerAttachment : IPlacemarkerAttachment {

        private Engine E;
        private IBoardObject attachedObj;

        private BoardManager BoardManager;
        private TurnController BoardController;
        private RulePanelManager RulePanelManager;
        private NotificationManager NotificationManager;
        private ChessSettingsController CommonSettingsController;

        private Notification currentNotification;

        public void SetEngine(Engine E) {
            Assert.NotNull("E", E);
            this.E = E;
            BoardManager = E.GetManager<BoardManager>();
            RulePanelManager = E.GetManager<RulePanelManager>();
            BoardController = E.GetController<TurnController>();
            NotificationManager = E.GetManager<NotificationManager>();
            CommonSettingsController = E.GetController<ChessSettingsController>();
        }

        public override void SetBoardObject(IBoardObject value) {
            attachedObj = value;
        }

        public override IBoardObject GetBoardObject() {
            return attachedObj;
        }

        public override bool HasBoardObject() {
            return attachedObj != null;
        }

        public override GameObject GetGameObject() {
            return gameObject;
        }

        public override void OnMouseDown() {
        }

        public override void OnMouseUp() {
            if (BoardController.GameAcceptingUserInput) {
                BoardManager.PlaceNewObject();   
            }
        }

        public override void OnMouseEnter() {
            Assert.NotNull("obj", attachedObj);
            if (BoardController.GameAcceptingUserInput) {
                if (attachedObj is Position) {

                    Position p = attachedObj as Position;
                    if (p.placingObject is Tile) {
                        E.EffectFactory.EnqueueNewEffect<IInstantMoveBoardObjectEffect>(E, p.placingObject, attachedObj.GetOffset_TS());
                        E.EffectFactory.EnqueueNewEffect<IAddTileEffect>(p.placingObject as Tile);
                    } else 
                        Assert.Never("Unknown placemarker type");
                        
                } else
                    Assert.Never("Unknown placemarker type");
            }
            if (!RulePanelManager.MainRulePanel.IsOpen) {
                currentNotification = new Notification() {
                    Text = "Place new object at " + attachedObj.GetOffset_TS()
                };
                NotificationManager.ShowNotification_TS(currentNotification);
            }
        }
        
        public void OnMouseOver() {
        }

        public void OnMouseExit() {
            if (!RulePanelManager.MainRulePanel.IsOpen) {
                if (attachedObj is Position) {

                    Position p = attachedObj as Position;
                    if (p.placingObject is Tile) {
                        E.EffectFactory.EnqueueNewEffect<IRemoveTileEffect>(p.placingObject as Tile);
                    } else
                        Assert.Never("Unknown placemarker type");

                } else
                    Assert.Never("Unknown placemarker type");

                if (currentNotification != null) {
                    NotificationManager.HideNotification_TS(currentNotification);
                    currentNotification = null;
                }
            }
        }

        public override void Hover() {
            
        }

        public override void Unhover() {
            
        }

        public override void PossibleAction() {
            gameObject.GetComponent<Renderer>().material = CommonSettingsController.BoardTemplates.PossibleActionMaterial;
            gameObject.transform.localPosition = Vector3.zero;
        }

        public override void UnpossibleAction() {
            gameObject.GetComponent<Renderer>().material = CommonSettingsController.BoardTemplates.UnpossibleActionMaterial;
        }
        
    }
}