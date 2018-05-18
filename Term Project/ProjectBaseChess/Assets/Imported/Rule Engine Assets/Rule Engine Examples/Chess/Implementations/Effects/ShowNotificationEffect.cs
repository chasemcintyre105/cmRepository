using RuleEngine;
using RuleEngineAddons.RulePanel;

namespace RuleEngineExamples.Chess {

    public class ShowNotificationEffect : IShowNotificationEffect {

        public Notification notification;

        public override Effect Init(params object[] parameters) {
            notification = (Notification) parameters[0];
            return this;
        }

        public override void Apply() {
            RuleEngineController.E.GetManager<NotificationManager>().ShowNotification(notification);
        }

        public override object[] GetEffectData() {
            return new object[] { notification };
        }

    }

}
