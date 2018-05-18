using RuleEngine;
using RuleEngineAddons.TurnBased;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuleEngineAddons.RulePanel {

    public class ButtonAttachment : MonoBehaviour, DragAndDropActionable, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerExitHandler, IPointerEnterHandler {

        public Rule associatedRule;
        public RuleComponent associatedRuleComponent;
		public ArgumentAccessor associatedRuleComponentAccessor;
		public GUIEvent command;
		public Dictionary<string, object> properties { get; private set; }

        private Notification currentNotification;

		private Image imageComponent;
		private Color NormalColour;
		private Color PossibleColour;
		private Color HoverColour;

        private TurnController _TurnController;
        private TurnController TurnController {
            get {
                if (_TurnController == null)
                    _TurnController = RuleEngineController.E.GetController<TurnController>();

                return _TurnController;
            }
        }

        private PanelManager _PanelManager;
        private PanelManager PanelManager {
            get {
                if (_PanelManager == null)
                    _PanelManager = RuleEngineController.E.GetManager<PanelManager>();

                return _PanelManager;
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

        public ButtonAttachment() {
			properties = new Dictionary<string, object>();
		}

		public void ButtonClickHandler() {
			if (TurnController != null && TurnController.GameAcceptingUserInput)
				PanelManager.RuleObjectDraggerDropper.CancelDragging();

            PanelManager.GUIStateMachine.IssueCommand(command, properties);
		}

		void Start() {

			imageComponent = gameObject.GetComponent<Image>();
			if (imageComponent != null) {
				NormalColour = imageComponent.color;
				PossibleColour = new Color(0f, 0f, 0.5f, 0.2f);
				HoverColour = new Color(0.4f, 0f, 0f, 0.2f);
			}

		}
        
		public void Hover() {
			if (associatedRuleComponent != null) {
				imageComponent.color = HoverColour;
			}
		}

		public void Unhover() {
			if (associatedRuleComponent != null) {
				imageComponent.color = NormalColour;
			}
		}
        
		public void PossibleAction() {
			if (associatedRuleComponent != null) {
				imageComponent.color = PossibleColour;
			}
		}
		
		public void UnpossibleAction() {
			if (associatedRuleComponent != null) {
				imageComponent.color = NormalColour;
			}
		}

		public void OnDrag(PointerEventData eventData) {
		}
		
		public void OnBeginDrag(PointerEventData eventData) {
            PanelManager.RuleObjectDraggerDropper.StartDragging(this, this, gameObject);
		}
		
		public void OnEndDrag(PointerEventData eventData) {
            PanelManager.RuleObjectDraggerDropper.StopDragging();
		}

		public void OnPointerEnter(PointerEventData eventData) {
            PanelManager.RuleObjectDraggerDropper.MouseEnteredDroppableObject(this, this);
            
            string text = null;

            if (associatedRuleComponent != null && associatedRuleComponent.Is<Statement>())
                text = associatedRuleComponent.As<Statement>().GetDescription();
            else if (associatedRuleComponentAccessor != null)
                text = associatedRuleComponentAccessor.GetArgumentObject().InterfaceDescription;
            else if (command == GUIEvent.Add_Rule)
                text = "Add a new rule";
            else if (command == GUIEvent.Add_Statement)
                text = "Add a new statement";
            else if (command == GUIEvent.Remove_Rule)
                text = "Remove this rule";

            if (text != null) {
                currentNotification = new Notification();
                currentNotification.Text = text;
                NotificationManager.ShowNotification_TS(currentNotification);
            }

		}

		public void OnPointerExit(PointerEventData eventData) {
            if (currentNotification != null) {
                NotificationManager.HideNotification_TS(currentNotification);
                currentNotification = null;
            }
        }

	}

}