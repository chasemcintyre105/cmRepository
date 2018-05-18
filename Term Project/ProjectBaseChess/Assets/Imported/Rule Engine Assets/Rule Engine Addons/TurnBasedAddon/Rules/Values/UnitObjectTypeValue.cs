using System;
using System.Collections.Generic;
using UnityEngine;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

	public class UnitObjectTypeValue : ObjectTypeValue {

        private UnitObjectRegistry _UnitObjectRegistry;
        protected UnitObjectRegistry UnitObjectRegistry {
            get {
                if (_UnitObjectRegistry == null)
                    _UnitObjectRegistry = E.ObjectRegistries[typeof(Unit)] as UnitObjectRegistry;

                return _UnitObjectRegistry;
            }
        }
        
		public override string GetSelectionPanelCategory() {
			return "Unit";
		}
        
		public override bool IsEqualTo (Value value) {
			if (value.GetType() != GetType())
				return false;

			UnitObjectTypeValue otherObject = (UnitObjectTypeValue) value;

			return otherObject.Name == Name;
		}

		public UnitObjectTypeValue(Engine E, UnitType UnitType) : base(E, UnitType) {
        }

        public override List<ObjectValue> GetAllInstances() {
            return UnitObjectRegistry.GenerateListOfNewBaseObjectValuesByType(TypeInstance as UnitType);
        }

        public override string GetDescription() {
			return "The type of a unit. In this case " + TypeInstance;
		}

        public override string ToString() {
            return TypeInstance.GetID();
        }

    }

}