using RuleEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuleEngineAddons.RulePanel {

    public class HoverNotificationAttachment : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {

        public string Tooltip;
        private Notification notification;

        private NotificationManager _NotificationManager;
        private NotificationManager NotificationManager {
            get {
                if (_NotificationManager == null)
                    _NotificationManager = RuleEngineController.E.GetManager<NotificationManager>();

                return _NotificationManager;
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (notification == null) {
                notification = new NotificationCreator(RuleEngineController.E)
                    .SetText(Tooltip)
                    .SetTriggered()
                    .SetPositioning(NotificationPositioning.GameObject)
                    .SetPositioningTarget(gameObject)
                    .Finalise();
            }
            NotificationManager.ShowNotification_TS(notification);
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (notification != null) {
                NotificationManager.HideNotification_TS(notification);
                notification = null;
            }
        }

    }

}