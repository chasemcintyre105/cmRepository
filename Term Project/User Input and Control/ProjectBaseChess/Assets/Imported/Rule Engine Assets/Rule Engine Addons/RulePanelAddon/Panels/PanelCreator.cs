using RuleEngine;
using System;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {

	public class PanelCreator {

        private PanelManager PanelManager;

        private GameObject newPanel;
        private RectTransform transform;

        public PanelCreator(Engine E) {
            PanelManager = E.GetManager<PanelManager>();
            newPanel = new GameObject();
            transform = newPanel.AddComponent<RectTransform>();
        }

        public PanelCreator(PanelManager PanelManager) {
            this.PanelManager = PanelManager;
            newPanel = new GameObject();
            transform = newPanel.AddComponent<RectTransform>();
        }

        public PanelCreator(PanelManager PanelManager, GameObject existingObject) {
            this.PanelManager = PanelManager;
            newPanel = existingObject;
            transform = newPanel.GetComponent<RectTransform>();
            if (transform == null)
                transform = newPanel.AddComponent<RectTransform>();
        }

        public PanelCreator(Engine E, GameObject existingObject) {
            PanelManager = E.GetManager<PanelManager>();
            newPanel = existingObject;
            transform = newPanel.GetComponent<RectTransform>();
            if (transform == null)
                transform = newPanel.AddComponent<RectTransform>();
        }

        public PanelCreator SetName(string name) {
            newPanel.name = name;
            return this;
        }

        public PanelCreator SetParent(GameObject parent) {
            transform.SetParent(parent.transform);
            return this;
        }

        public PanelCreator SetParent(Transform parent) {
            transform.SetParent(parent);
            return this;
        }

        public PanelCreator SetToFillParent() {
            PanelManager.SetChildPanelToFillPanel(newPanel, newPanel.transform.parent.gameObject);
            return this;
        }
        
        public PanelCreator SetChild(GameObject child) {
            child.transform.SetParent(transform);
            return this;
        }

        public PanelCreator SetPosition(Vector2 position) {
            transform.localPosition = position;
            return this;
        }

        public PanelCreator SetAbsolutePosition(Vector2 position) {
            transform.position = position;
            return this;
        }

        public PanelCreator SetPivot(Vector2 pivot) {
            transform.pivot = pivot;
            return this;
        }

        public PanelCreator SetPivot(float x, float y) {
            transform.pivot = new Vector2(x, y);
            return this;
        }

        public PanelCreator SetAnchors(Vector2 anchorMin, Vector2 anchorMax) {
            transform.anchorMin = anchorMin;
            transform.anchorMax = anchorMax;
            return this;
        }

        public PanelCreator SetAnchors(float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY) {
            transform.anchorMin = new Vector2(anchorMinX, anchorMinY);
            transform.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            return this;
        }

        public PanelCreator AddComponent<C>() where C : Component {
            newPanel.AddComponent<C>();
            return this;
        }

        public PanelCreator GetComponent<C>(out C component) where C : Component {
            component = newPanel.GetComponent<C>();
            return this;
        }

        public PanelCreator AddComponent<C>(Action<C> callback) where C : Component {
            callback.Invoke(newPanel.AddComponent<C>());
            return this;
        }

        public PanelCreator GetComponent<C>(out C component, Action<C> callback) where C : Component {
            component = newPanel.GetComponent<C>();
            callback.Invoke(component);
            return this;
        }

        public PanelCreator GetComponent<C>(Action<C> callback) where C : Component {
            callback.Invoke(newPanel.GetComponent<C>());
            return this;
        }

        public PanelCreator SafelyGetComponent<C>() where C : Component {
            if (newPanel.GetComponent<C>() == null)
                newPanel.AddComponent<C>();
            return this;
        }

        public PanelCreator SafelyGetComponent<C>(Action<C> callback) where C : Component {
            C component = newPanel.GetComponent<C>();
            if (component == null)
                component = newPanel.AddComponent<C>();
            callback.Invoke(component);
            return this;
        }

        public PanelCreator SafelyGetComponent<C>(out C component) where C : Component {
            component = newPanel.GetComponent<C>();
            if (component == null)
                component = newPanel.AddComponent<C>();
            return this;
        }

        public PanelCreator SafelyGetComponent<C>(out C component, Action<C> callback) where C : Component {
            component = newPanel.GetComponent<C>();
            if (component == null)
                component = newPanel.AddComponent<C>();
            callback.Invoke(component);
            return this;
        }

        public PanelCreator GetRectTransform(out RectTransform rectTransform) {
            rectTransform = transform;
            return this;
        }

        public PanelCreator GetRectTransform(Action<RectTransform> callback) {
            callback.Invoke(transform);
            return this;
        }

        public PanelCreator GetRectTransform(out RectTransform rectTransform, Action<RectTransform> callback) {
            rectTransform = transform;
            callback.Invoke(transform);
            return this;
        }

        public PanelCreator ConfigureWithDrawingAttachment() {
            PanelManager.ConfigurePanelInstance(newPanel);
            return this;
        }

        public PanelCreator ConfigureWithDrawingAttachment(out PanelDrawingAttachment attachment) {
            attachment = PanelManager.ConfigurePanelInstance(newPanel);
            return this;
        }

        public PanelCreator SetAsFullScreen(Canvas parentCanvas) {
            PanelManager.SetPanelAsFullscreen(newPanel, parentCanvas);
            return this;
        }

        public PanelCreator SetVisibility(bool visible) {
            newPanel.SetActive(visible);
            return this;
        }

        public void Finalise(out GameObject panel) {
            panel = newPanel;
            Finalise();
        }

        public void Finalise() {

        }

    }

}

