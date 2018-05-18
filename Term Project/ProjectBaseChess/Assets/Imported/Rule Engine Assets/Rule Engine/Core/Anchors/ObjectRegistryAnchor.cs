using System;

namespace RuleEngine {

	public class ObjectRegistryAnchor : Anchor {

        public ObjectRegistryAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }
        
        public SpecificObjectRegistry<O, OT, OV, OTV> RegisterNewObjectRegistry<O, OT, OV, OTV>(SpecificObjectRegistry<O, OT, OV, OTV>.ObjectValueCreator NewObjectValue, 
                                                                                                SpecificObjectRegistry<O, OT, OV, OTV>.ObjectTypeValueCreator NewObjectTypeValue) 
                                                                                                        where O : IObject
                                                                                                        where OT : IObjectType
                                                                                                        where OV : ObjectValue
                                                                                                        where OTV : ObjectTypeValue {

            return initialiser.GetEngine().RegisterNewObjectRegistry<O, OT, OV, OTV>(NewObjectValue, NewObjectTypeValue);
        }

        public void RegisterNewObjectRegistry(ObjectRegistry ObjectRegistry) {
            initialiser.GetEngine().RegisterNewObjectRegistry(ObjectRegistry);
        }
        
        public override string GetDescription() {
            return "An anchor that allows for the creation and management of object registries.";
        }

    }

}