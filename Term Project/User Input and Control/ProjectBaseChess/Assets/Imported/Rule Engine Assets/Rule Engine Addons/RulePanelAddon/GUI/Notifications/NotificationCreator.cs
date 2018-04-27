using RuleEngine;
using System;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {
	
	public class NotificationCreator {

        private Notification newNotification;
        
		public NotificationCreator(Engine E) {
            MakeAndInitialiseNotification();
		}

        public NotificationCreator SetText(string text) {
            newNotification.Text = text;
            return this;
        }

        public NotificationCreator SetHover(GameObject target) {
            Assert.Same("Notification type is unset", newNotification.Type, NotificationType.Unset);
            newNotification.Type = NotificationType.MouseOver;
            newNotification.TargetType = NotificationTargetType.GameObject;
            newNotification.TargetGameObject = target;
            return this;
        }
        
        public NotificationCreator SetTriggered() {
            Assert.Same("Notification type is unset", newNotification.Type, NotificationType.Unset);
            newNotification.Type = NotificationType.Triggered;
            return this;
        }

        public NotificationCreator SetPositioning(NotificationPositioning positioning) { 
            newNotification.Positioning = positioning;
            return this;
        }

        public NotificationCreator SetPositioningTarget(GameObject target) {
            Assert.Same("Notification type is unset", newNotification.Type, NotificationType.Triggered);
            newNotification.TargetGameObject = target;
            return this;
        }

        public NotificationCreator SetDelay(float delayInSeconds) {
            Assert.Same("Notification type is triggered", newNotification.Type, NotificationType.Triggered);
            newNotification.DelayBeforeDisplayInSeconds = delayInSeconds;
            return this;
        }

        public NotificationCreator SetDisplayPeriod(float periodInSeconds) {
            Assert.Same("Notification type is triggered", newNotification.Type, NotificationType.Triggered);
            newNotification.DisplayPeriodInSeconds = periodInSeconds;
            return this;
        }


        public NotificationCreator SetCallbackForTrigger(Action callback) {
            Assert.Same("Notification type is triggered", newNotification.Type, NotificationType.Triggered);
            newNotification.CallbackOnTrigger = callback;
            return this;
        }
        
        public NotificationCreator SetCallbackForFailure(Action callback) {
            Assert.Same("Notification type is triggered", newNotification.Type, NotificationType.Triggered);
            newNotification.CallbackOnFailure = callback;
            return this;
        }

        public NotificationCreator SetCallbackForDisparition(Action callback) {
            newNotification.CallbackOnDisparition = callback;
            return this;
		}
        
        public void Finalise(out Notification notification) {
            notification = newNotification;
			Finalise();
		}

		public Notification Finalise() {

            Assert.True("Type is set", newNotification.Type != NotificationType.Unset);
            if (newNotification.Type == NotificationType.Triggered) {
                Assert.True("Delay is valid", newNotification.DelayBeforeDisplayInSeconds >= 0f);
            } else if (newNotification.Type == NotificationType.MouseOver) {
                Assert.True("Target type is set", newNotification.TargetType != NotificationTargetType.None || newNotification.TargetType != NotificationTargetType.Unset);
                Assert.True("Target is set", newNotification.TargetGameObject != null);
            } else {
                Assert.Never("Notification type not yep implemented");
            }
            
            Assert.True("Positioning type is set", newNotification.Positioning != NotificationPositioning.Unset);
            if (newNotification.Positioning == NotificationPositioning.Screen) {
                Assert.True("Position is set", newNotification.ScreenPosition.HasValue);
            }

            return newNotification;
        }

        private void MakeAndInitialiseNotification() {
            newNotification = new Notification();
        }

	}
	
}
