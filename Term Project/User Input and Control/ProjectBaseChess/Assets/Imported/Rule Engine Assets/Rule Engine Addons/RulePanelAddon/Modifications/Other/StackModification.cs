using RuleEngine;

namespace RuleEngineAddons.RulePanel {

    public abstract class StackModification : Modification {

        protected Engine E;

        public StackModification(Engine E) {
            this.E = E;
        }

    }

}

