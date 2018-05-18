using RuleEngine;
using RuleEngineAddons.RulePanel;

namespace RuleEngineExamples.Chess {

	public class DisplayMessageEffect : IDisplayMessageEffect {

        public string message;

		public override Effect Init(params object[] parameters) {
			message = (string) parameters[0];
            return this;
        }

		public override void Apply() {
            RuleEngineController.E.GetManager<NotificationManager>().ShowNotification_TS(new Notification() {
                DisplayPeriodInSeconds = 4,
                Text = message
            });
        }

        public override object[] GetEffectData() {
            return new object[] { message };
        }

    }

}
