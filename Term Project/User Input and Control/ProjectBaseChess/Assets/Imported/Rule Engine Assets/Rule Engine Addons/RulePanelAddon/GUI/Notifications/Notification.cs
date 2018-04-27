using System;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {
    
    public class Notification {
        
        public NotificationType Type;
        public NotificationTargetType TargetType = NotificationTargetType.None;
        public NotificationPositioning Positioning;

        // Text to display
        public string Text;
        public Color TextColor = Color.white;

        // Game/Menu
        public GameObject TargetGameObject = null;

        // Event
        public Action TriggerEvent = null;
        public Action CallbackOnTrigger = null;
        public Action CallbackOnFailure = null;
        public Action CallbackOnDisparition = null;

        public float DelayBeforeDisplayInSeconds = 0f;
        public float DisplayPeriodInSeconds = 0f;

        // Positioning
        public Vector2? ScreenPosition = null;

        // Visibility
        public bool Priority = false;

        // Run time variables
        public bool visible = false;

        private bool eventCancelled = false;
        public void CancelEvent() {
            eventCancelled = true;
        }
        public bool IsEventCancelled() {
            return eventCancelled;
        }

        public override string ToString() {
            return Text;
        }
    }

}