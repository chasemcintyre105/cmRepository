namespace RuleEngine {

    public class Creator {

        protected Engine E;
        protected Creator Parent;

        public Creator(Engine E, Creator Parent) {
            this.E = E;
            this.Parent = Parent;
        }
        
    }

}