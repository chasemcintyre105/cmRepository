using System;

namespace RuleEngine {

    public class CRENullWrapper : CREValueWrapper {
        
        public CRENullWrapper(Engine E) : base(E, NullValue.Instance) {
        }
        
        public override Type GetValueType() {
            return NullValue.Instance.GetType();
        }

        public override O CalculateValue<O>(RuleExecutor RuleExecutor) {
            RuleExecutor.ExecuteCompiledVisitNullValue(RuleExecutor, NullValue.Instance);
            return NullValue.Instance as O;
        }

    }

}
