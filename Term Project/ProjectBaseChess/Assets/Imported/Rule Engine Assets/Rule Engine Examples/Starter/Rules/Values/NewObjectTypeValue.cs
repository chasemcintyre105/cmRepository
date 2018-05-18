using System;
using System.Collections.Generic;
using RuleEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewObjectTypeValue : ObjectTypeValue {

        NewObjectRegistry newObjectRegistry;
        
		public override string GetSelectionPanelCategory() {
			return "Object Types";
		}
        
		public override bool IsEqualTo (Value value) {
			if (value.GetType() != GetType())
				return false;

			NewObjectTypeValue otherObject = (NewObjectTypeValue) value;

			return otherObject.Name == Name;
		}

		public NewObjectTypeValue(Engine E, NewObjectType TypeInstance) : base(E, TypeInstance) {
            newObjectRegistry = (NewObjectRegistry) E.ObjectRegistries[typeof(NewObject)];
        }
        
		public override string GetDescription() {
			return "Represents an object type.";

        }

        public override string ToString() {
            return "NewObjectTypeValue: " + TypeInstance;
        }

        public override List<ObjectValue> GetAllInstances() {
            return newObjectRegistry.GenerateListOfNewBaseObjectValues();
        }

    }

}