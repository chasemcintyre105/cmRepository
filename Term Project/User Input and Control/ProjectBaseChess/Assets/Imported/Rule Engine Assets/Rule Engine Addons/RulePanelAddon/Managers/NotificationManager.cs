using RuleEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuleEngineAddons.RulePanel {

    public class NotificationManager : IManager {

        protected RulePanelController RulePanelController;

        protected Notification CurrentlyDisplayingNotification;
        protected List<Notification> QueuedNotifications = new List<Notification>();
        protected List<Notification> CancelledNotifications = new List<Notification>();

        public override void Preinit() {
        }

        public override void Init() {
            RulePanelController = E.GetController<RulePanelController>();

            RectTransform CanvasRect = RulePanelController.OrganisationalObjects.Canvas.transform as RectTransform;

            GameObject tooltipContainer = GameObject.Instantiate(RulePanelController.GUITemplates.panelTemplate);
            RulePanelController.OrganisationalObjects.Tooltip = tooltipContainer;
            RectTransform tooltipContainerRect = E.GetManager<PanelManager>().GetRectTransform(tooltipContainer);
            tooltipContainer.name = "Tooltip";
            tooltipContainer.transform.SetParent(RulePanelController.OrganisationalObjects.Canvas.transform);

            tooltipContainerRect.sizeDelta = new Vector2(0, -CanvasRect.rect.height + 50);
            tooltipContainerRect.localPosition = new Vector2(-CanvasRect.rect.width / 2, CanvasRect.rect.height / 2);
            tooltipContainerRect.pivot = new Vector2(0.5f, 0f);

            GameObject tooltip = new GameObject("Text");
            tooltip.transform.SetParent(tooltipContainer.transform);
            RectTransform tooltipRect = tooltip.AddComponent<RectTransform>();

            Text tooltipText = tooltip.AddComponent<Text>();
            RulePanelController.OrganisationalObjects.TooltipText = tooltipText;
            tooltipText.alignment = TextAnchor.MiddleCenter;
            tooltipText.resizeTextForBestFit = false;
            tooltipText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            tooltipText.fontSize = 17;
            tooltipText.color = Color.white;

            tooltipRect.anchorMin = Vector2.zero;
            tooltipRect.anchorMax = Vector2.one;
            tooltipRect.localPosition = new Vector2(0, tooltipContainerRect.rect.height / 2);
            tooltipRect.sizeDelta = Vector2.zero;

            // Disable the tooptip on load
            tooltipContainer.SetActive(false);

        }

        public virtual void ShowNotification_TS(Notification notification) {

            // Enqueue the notification with a delay if given
            if (notification.DelayBeforeDisplayInSeconds > 0f) {
                E.EffectFactory.EnqueueNewEffectWithDelay<IShowNotificationEffect>(notification.DelayBeforeDisplayInSeconds, notification);
                QueuedNotifications.Add(notification);
            } else {
                notification.DelayBeforeDisplayInSeconds = 0f; // Make sure that the delay is zero in this case
                E.EffectFactory.EnqueueNewEffect<IShowNotificationEffect>(notification);
            }

            // Enqueue a hide effect for the same notification if a display period is given
            if (notification.DisplayPeriodInSeconds > 0f) {
                E.EffectFactory.EnqueueNewEffectWithDelay<IHideNotificationEffect>(notification.DelayBeforeDisplayInSeconds + notification.DisplayPeriodInSeconds, notification);
            }

        }

        // Hide a notification without delay
        public virtual void HideNotification_TS(Notification notification) {
            E.EffectFactory.EnqueueNewEffect<IHideNotificationEffect>(notification);
        }

        public virtual void ShowNotification(Notification notification) {

            if (notification == null)
                throw new Exception("Notification was null");

            if (notification.Priority) {

                // Show
                SetNotification(notification);

            } else {

                if (CurrentlyDisplayingNotification == null) {

                    // Show
                    SetNotification(notification);

                } else if (!CurrentlyDisplayingNotification.Priority) {

                    // Show
                    SetNotification(notification);

                }

            }

        }

        public virtual void HideNotification(Notification notification) {

            if (notification == null)
                throw new Exception("Notification was null");

            if (CurrentlyDisplayingNotification == notification) {
                UnsetNotification();
                CurrentlyDisplayingNotification = null;
            } else if (QueuedNotifications.Contains(notification)) {
                QueuedNotifications.Remove(notification);
                CancelledNotifications.Add(notification);
            }

        }

        public virtual void ClearNotifications() {
            UnsetNotification();

            CancelledNotifications.Clear();
            CancelledNotifications.AddRange(QueuedNotifications);
            QueuedNotifications.Clear();
        }

        protected virtual void SetNotification(Notification notification) {

            // Remove from queue if necessary
            if (QueuedNotifications.Contains(CurrentlyDisplayingNotification))
                QueuedNotifications.Remove(CurrentlyDisplayingNotification);

            // Unshow previous notification
            if (CurrentlyDisplayingNotification != null)
                CurrentlyDisplayingNotification.visible = false;

            // Set new notification
            CurrentlyDisplayingNotification = notification;
            CurrentlyDisplayingNotification.visible = true;

            // Set text of new notification
            if (notification.Text != "" && RulePanelController.OrganisationalObjects.Tooltip != null) {
                RulePanelController.OrganisationalObjects.Tooltip.SetActive(true);
                RulePanelController.OrganisationalObjects.TooltipText.text = notification.Text;

                // Set colour
                RulePanelController.OrganisationalObjects.TooltipText.color = notification.TextColor;

            }

        }

        protected virtual void UnsetNotification() {

            // Remove from queue if necessary
            if (QueuedNotifications.Contains(CurrentlyDisplayingNotification))
                QueuedNotifications.Remove(CurrentlyDisplayingNotification);

            // Disable the tooltip object
            if (RulePanelController.OrganisationalObjects.Tooltip != null)
                RulePanelController.OrganisationalObjects.Tooltip.SetActive(false);

        }

    }

}