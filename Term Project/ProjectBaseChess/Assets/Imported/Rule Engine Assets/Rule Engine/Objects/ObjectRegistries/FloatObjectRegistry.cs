namespace RuleEngine {

    public class FloatObjectRegistry : SpecificObjectRegistry<Float, NumberType, NumberValue, NumberTypeValue> {
        
        private static ObjectValueCreator CreateFloatValue = delegate (Engine E, Float o, NumberTypeValue otv) {
            return new NumberValue(E, otv, o);
        };

        private static ObjectTypeValueCreator CreateFloatTypeValue = delegate (Engine E, NumberType ot) {
            return new NumberTypeValue(E, ot);
        };

        public FloatObjectRegistry(Engine E) : base (E, CreateFloatValue, CreateFloatTypeValue) {
            FloatType = new NumberType("Float");
        }

        public NumberType FloatType;

        private int floatIDTicker = 0;

        protected virtual int GenerateFloatUID_TS() {
            lock (_registryLock) {
                return floatIDTicker++;
            }
        }
        
        public Float GetFloat(float n) {
            lock (_registryLock) {
                return new Float(GenerateFloatUID_TS(), n, FloatType);
            }
        }
        
    }

}
