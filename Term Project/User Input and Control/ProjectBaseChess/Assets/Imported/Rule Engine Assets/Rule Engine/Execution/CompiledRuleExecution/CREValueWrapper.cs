using System;
using UnityEngine;

namespace RuleEngine {

    public class CREValueWrapper : CREValue {

        public static CREValueWrapper Null;
        public static CREValueWrapper True;
        public static CREValueWrapper False;
        
        public static CREValueWrapper GetValueWrapper(Engine E, Value value) {

            if (value is NullValue) {
                if (Null == null)
                    Null = new CRENullWrapper(E);

                return Null;
            }
            
            // Check for duplicates
            if (value is BooleanValue) {
                if ((value as BooleanValue).value) {

                    if (True == null)
                        True = new CREValueWrapper(E, value);

                    return True;

                } else {

                    if (False == null)
                        False = new CREValueWrapper(E, value);

                    return False;

                }
            }

            if (value is Variable) {
                return new CREVariableWrapper(E, value as Variable);
            }

            return new CREValueWrapper(E, value);
        }
        
        protected CREValueWrapper(Engine E, Value value) : base(E, value) {
        }

        public override Type GetValueType() {
            return associatedObj.GetType();
        }

        public override O CalculateValue<O>(RuleExecutor RuleExecutor) {
            RuleExecutor.ExecuteCompiledNonNullValue(RuleExecutor, associatedObj as Value);

            if (!(associatedObj is O))
                throw new Exception("The assumed type (" + typeof(O).Name + ") is not the type of the value (" + associatedObj.GetType().Name + ").");

            return associatedObj as O;
        }

    }

}
