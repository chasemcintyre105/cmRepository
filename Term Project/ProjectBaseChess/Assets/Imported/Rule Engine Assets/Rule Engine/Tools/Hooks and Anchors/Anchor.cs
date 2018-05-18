namespace RuleEngine {

    public abstract class Anchor {

        public abstract void Init();
        public abstract string GetDescription();

        protected RuleEngineInitialiser initialiser;

        public Anchor(RuleEngineInitialiser initialiser) {
            this.initialiser = initialiser;
        }

    }

}