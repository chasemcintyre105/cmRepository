using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class RulePanelSelectableObjectRegistrationAnchor : Anchor {

        private RulePanelManager RulePanelManager;

        public RulePanelSelectableObjectRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
            RulePanelManager = initialiser.GetEngine().GetManager<RulePanelManager>();
        }
        
        public void RegisterSelectableObject(RuleComponent obj) {
            RulePanelManager.RegisterSelectableObject(obj);
        }

        public override string GetDescription() {
            return "An anchor that allows for registration of selectable objects in rule panels.";
        }

    }

}