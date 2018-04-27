using RuleEngine;
using RuleEngineAddons.RulePanel;
using System;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class RuleExceptionEffect : IRuleExceptionEffect {

        public string type;
        public string message;
        public string stacktrace = null;

        public override Effect Init(params object[] parameters) {
            type = (string) parameters[0];
            message = (string) parameters[1];
            if (parameters.Length > 2)
                stacktrace = (string) parameters[2];
            return this;
        }

        public override void Apply() {
			if (type == "Error") { // Serious errors
                Debug.LogError(message + (stacktrace != null ? "\n" + stacktrace : ""));

                // Do something else here depending on the desired behaviour (report error, restart...)

            } else if (type == "RuleError") { // Errors produced as a result of the rules
                RuleEngineController.E.GetManager<NotificationManager>().ShowNotification_TS(new Notification() {
                    DisplayPeriodInSeconds = 3,
                    Text = message,
                    Type = NotificationType.Triggered,
                    TextColor = Color.red,
                    Priority = true
                });
            }
		}

        public override object[] GetEffectData() {
            return new object[] { type, message, stacktrace };
        }

    }

}
