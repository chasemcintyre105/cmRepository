namespace RuleEngine {

    public class IntegerObjectRegistry : SpecificObjectRegistry<Integer, NumberType, NumberValue, NumberTypeValue> {

        // Cached values
        private Integer[] positiveIntegers = new Integer[1000]; // 0 to 999
        private Integer[] negativeIntegers = new Integer[1000]; // -1 to -1000

        private static ObjectValueCreator CreateIntegerValue = delegate (Engine E, Integer o, NumberTypeValue otv) {
            return new NumberValue(E, otv, o);
        };

        private static ObjectTypeValueCreator CreateIntegerTypeValue = delegate (Engine E, NumberType ot) {
            return new NumberTypeValue(E, ot);
        };

        public IntegerObjectRegistry(Engine E) : base (E, CreateIntegerValue, CreateIntegerTypeValue) {
            IntegerType = new NumberType("Integer");
        }

        public NumberType IntegerType { get; private set; }
        
        public Integer GetInteger_TS(int n) {
            Integer newInt = null;
            bool needsRegistering = false;

            lock (_registryLock) {
                if (n >= 0) {
                    if (n < 1000) {
                        newInt = positiveIntegers[n];
                        if (newInt == null) {
                            newInt = new Integer(n, IntegerType);
                            needsRegistering = true;
                            positiveIntegers[n] = newInt;
                        }
                    } else {
                        needsRegistering = true;
                        newInt = new Integer(n, IntegerType);
                    }
                } else {
                    if (n >= -1000) {
                        int nni = -n - 1;
                        newInt = negativeIntegers[nni];
                        if (newInt == null) {
                            newInt = new Integer(n, IntegerType);
                            needsRegistering = true;
                            negativeIntegers[nni] = newInt;
                        }
                    } else { 
                        needsRegistering = true;
                        newInt = new Integer(n, IntegerType);
                    }
                }
            }

            if (needsRegistering)
                RegisterObject_TS(newInt);

            return newInt;
        }

        public NumberValue CreateIntegerNumberValueFromInt_TS(int n) {
            Integer integer = GetInteger_TS(n);
            return CreateObjectValue_TS(integer);
        }

    }

}
