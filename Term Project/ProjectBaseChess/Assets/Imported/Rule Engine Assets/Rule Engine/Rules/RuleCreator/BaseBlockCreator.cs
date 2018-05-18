namespace RuleEngine {

    public class BaseBlockCreator : BlockCreator {

        private RuleCreator RuleCreator;

        public BaseBlockCreator(Engine E, Block Block, RuleCreator RuleCreator) : base(E, null, Block) {
            this.RuleCreator = RuleCreator;
        }

        public new BaseBlockCreator Add<S>(params Value[] values) where S : Statement {
            AddAndConfigure<S>().Set(values).ReturnTo<BaseBlockCreator>();
            return this;
        }

        public new BaseBlockCreator Add<S>() where S : Statement {
            AddAndConfigure<S>().ReturnTo<BaseBlockCreator>();
            return this;
        }

        public RuleCreator FinishRule() {
            return RuleCreator;
        }

        public override string ToString() {
            return "BaseBlockCreator";
        }

    }

}
