using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public class ActionStackConfigurationAnchor : Anchor {

        public ActionStackManager ActionStackManager;

        public ActionStackConfigurationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
            ActionStackManager = initialiser.GetEngine().GetManager<ActionStackManager>();
        }
        
        public override string GetDescription() {
            return "An anchor that allows for registration of selectable objects in rule panels.";
        }

    }

}