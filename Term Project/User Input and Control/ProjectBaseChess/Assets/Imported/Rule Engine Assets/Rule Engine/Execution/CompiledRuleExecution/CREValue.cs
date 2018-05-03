using System;

namespace RuleEngine {

    public abstract class CREValue : CompiledRuleExecutable {
        
        public abstract Type GetValueType();

        public CREValue(Engine E, RuleComponent associatedObj) : base(E, associatedObj) {
        }
        
        public override bool IsValueType(Type type) {
            return GetValueType() == type;
        }

    }

}
