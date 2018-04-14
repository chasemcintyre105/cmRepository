using RuleEngine;
using RuleEngineAddons.RulePanel;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class HideNotificationEffect : IHideNotificationEffect {

        public Notification notification;
        
        public override Effect Init(params object[] parameters) {
            notification = (Notification) parameters[0];
            return this;
        }

		public override void Apply() {
            RuleEngineController.E.GetManager<NotificationManager>().HideNotification(notification);
        }

        public override object[] GetEffectData() {
            return new object[] { notification };
        }

    }

}
