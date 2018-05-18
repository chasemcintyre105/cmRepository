
namespace RuleEngine {

    public class CREArgument {

        public string name { get; private set; }
        private CompiledRuleExecutable value;
        public RuleComponent parent = null;
        public int index = -1;

        public CREArgument(string name, CompiledRuleExecutable value) {
            this.name = name;
            this.value = value;
        }

        public O CalculateValue<O>(RuleExecutor RuleExecutor) where O : RuleComponent {
            RuleExecutor.visitedObjectsStack.Push(new RuleExecutor.VisitedObject(parent, index));
            O o = value.CalculateValue<O>(RuleExecutor);
            RuleExecutor.visitedObjectsStack.Pop();
            return o;
        }

    }

}
