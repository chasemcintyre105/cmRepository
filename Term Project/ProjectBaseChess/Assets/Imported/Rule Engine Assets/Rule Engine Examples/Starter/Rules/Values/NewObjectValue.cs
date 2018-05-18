using RuleEngine;
using System;

namespace RuleEngineExamples.StarterProject {

	public class NewObjectValue : ObjectValue {
        
		public override string GetSelectionPanelCategory() {
			return "Objects";
		}
        
        public NewObjectValue(Engine E, NewObjectTypeValue ObjTypeValue, NewObject Instance) : base(E, ObjTypeValue, Instance) {
        }
        
		public override string GetDescription() {
			return "Represents an object.";
		}

        public override string ToString() {
            return "NewObjectValue (" + Instance + ")";
        }

    }

}