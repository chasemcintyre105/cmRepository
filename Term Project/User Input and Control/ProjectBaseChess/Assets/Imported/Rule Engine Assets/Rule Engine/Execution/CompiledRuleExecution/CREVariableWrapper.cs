using System;

namespace RuleEngine {

    public class CREVariableWrapper : CREValueWrapper {

        public CREVariableWrapper(Engine E, Variable variable) : base(E, variable) {
        }
        
        public override Type GetValueType() {
            return (associatedObj as Variable).VariableValue.GetType();
        }

        public override O CalculateValue<O>(RuleExecutor RuleExecutor) {
            Value variableValue = (associatedObj as Variable).VariableValue;

            if (variableValue == NullValue.Instance) {
                RuleExecutor.ExecuteCompiledVisitNullValue(RuleExecutor, NullValue.Instance);
            } else {
                RuleExecutor.ExecuteCompiledNonNullValue(RuleExecutor, variableValue);
            }

            if (!(variableValue is O))
                throw new Exception("The requested type (" + typeof(O).Name + ") is not the type of the value (" + variableValue.GetType().Name + ").");

            return variableValue as O;
        }

    }

}
