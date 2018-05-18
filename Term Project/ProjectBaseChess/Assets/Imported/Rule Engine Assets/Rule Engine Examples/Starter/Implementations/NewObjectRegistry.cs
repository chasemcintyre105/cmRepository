using RuleEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewObjectRegistry : SpecificObjectRegistry<NewObject, NewObjectType, NewObjectValue, NewObjectTypeValue> {

        private static NewObjectValue CreateNewObjectValue(Engine E, NewObject o, NewObjectTypeValue otv) {
            return new NewObjectValue(E, otv, o);
        }

        private static NewObjectTypeValue CreateNewObjectTypeValue(Engine E, NewObjectType ot) {
            return new NewObjectTypeValue(E, ot);
        }
        
        public NewObjectRegistry(Engine E) : base (E, CreateNewObjectValue, CreateNewObjectTypeValue) {
        }
        
    }

}

