using RuleEngine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RuleEngineAddons.RulePanel {

    public class RulePanelRenderer : MonoBehaviour {

        private RulePanelManager RulePanelManager;

        public string PanelName;

        [Serializable]
        public class RulePanelBehaviour {
            public bool Closable = true;
            public bool Scrollable = true;
            public bool IsOpenByDefault = true;
        }
        public RulePanelBehaviour Behaviour;

        [Serializable]
        public class RulePanelAppearance {
            public bool FullScreen = true;
            public Sprite Background;
            public Color Color = Color.white;
            public Scrollbar ScrollBarTemplate;
            public GameObject SelectionBackdropTemplate;
        }
        public RulePanelAppearance Appearance;
        
        public bool IsOpen { get; protected set; }
        public Canvas Canvas { get; protected set; }
        public Scrollbar Scrollbar { get; protected set; }
        public GameObject SelectionBackdrop { get; protected set; }

        public Action DecideToOpen;
        public Action DecideToClose;
        
        public void Init() {

            // Set the opposite of the default, so that it can be moved to the correct state
            if (Behaviour.IsOpenByDefault)
                IsOpen = false;
            else
                IsOpen = true;

        }

        public void SetCanvas(Canvas Canvas) {
            Assert.Null("Canvas has not yet been set", this.Canvas);
            this.Canvas = Canvas;
        }

        public void SetRulePanelManager(RulePanelManager RulePanelManager) {
            this.RulePanelManager = RulePanelManager;
        }

        public void SetScrollbar(Scrollbar Scrollbar) {
            Assert.Null("Scrollbar has not yet been set", this.Scrollbar);
            this.Scrollbar = Scrollbar;
        }

        public void SetSelectionBackdrop(GameObject SelectionBackdrop) {
            Assert.Null("SelectionBackdrop has not yet been set", this.SelectionBackdrop);
            this.SelectionBackdrop = SelectionBackdrop;
        }

        public void Redraw() {
            RulePanelManager.RefreshRulePanel(this);
        }

        public void ToggleVisibility() {
            if (IsOpen) {
                Close();
            } else {
                Open();
            }
        }

        public void Close() {
            Assert.True("Panel is open", IsOpen);

            if (DecideToClose == null) {
                ForceClose();
            } else {

                // Leave the opening process to somewhere else
                // If this function eventually decides to close the rule panel, it is to call ForceClose
                DecideToClose();

            }

        }

        public void Open() {
            Assert.False("Panel is not open", IsOpen);

            if (DecideToOpen == null) {
                ForceOpen();
            } else {

                // Leave the closing process to somewhere else
                // If this function eventually decides to open the rule panel, it is to call ForceOpen
                DecideToOpen();

            }

        }

        public void ForceOpen() {
            Assert.False("Panel is not open", IsOpen);

            // Declare the panel open
            IsOpen = true;
            RuleEngineController.E.EffectFactory.EnqueueNewEffect<IOpeningRulePanelEffect>(this);

        }

        public void ForceClose() {
            Assert.True("Panel is open", IsOpen);

            // Declare the panel closed
            IsOpen = false;
            RuleEngineController.E.EffectFactory.EnqueueNewEffect<IClosingRulePanelEffect>(this);

        }

    }

}


