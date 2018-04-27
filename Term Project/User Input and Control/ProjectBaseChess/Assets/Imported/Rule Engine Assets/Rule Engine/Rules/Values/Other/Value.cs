using System;

namespace RuleEngine {

	public abstract class Value : RuleComponent {
        
		public abstract bool IsEqualTo(Value value);

		public Value(Engine E) : base(E) {}
		
		public override void DefineArguments() {
            // Define no arguments for a value
		}

        public override CompiledRuleExecutable NewCRE() {
            return CREValueWrapper.GetValueWrapper(E, this);
        }

        public override Type GetReturnType() {
            return GetType();
        }

    }

}