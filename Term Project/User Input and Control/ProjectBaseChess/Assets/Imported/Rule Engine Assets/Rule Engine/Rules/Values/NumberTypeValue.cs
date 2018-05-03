using System;
using System.Collections.Generic;

namespace RuleEngine {

    public class NumberTypeValue : ObjectTypeValue {

        ObjectRegistry ObjectRegistry;

        public override string GetSelectionPanelCategory() {
			return "Number Types";
		}
        
		public override bool IsEqualTo(Value v) {

			if (v.GetType() != typeof(NumberTypeValue))
				return false;

			return ((NumberTypeValue) v).TypeInstance == TypeInstance;
		}

		public NumberTypeValue(Engine E, NumberType numberType) : base(E, numberType) {
            if (numberType.GetID() == "Integer") {
                ObjectRegistry = E.GetObjectRegistry<Integer>();
            } else if (numberType.GetID() == "Float") {
                ObjectRegistry = E.GetObjectRegistry<Float>();
            } else
                throw new Exception("Number type not recognised: " + numberType.GetID());
        }
		
		public override string GetDescription() {
			return "The number type " + TypeInstance;
		}

        public override string ToString() {
            return TypeInstance.ToString();
        }

        public override List<ObjectValue> GetAllInstances() {
            return ObjectRegistry.GenerateListOfNewBaseObjectValuesByTypeID(TypeInstance.GetID());
        }

    }

}