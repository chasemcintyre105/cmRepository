using System;
using System.Collections.Generic;
using UnityEngine;
using RuleEngine;

namespace RuleEngineAddons.TurnBased {

	public class PositionObjectTypeValue : ObjectTypeValue {

        private PositionObjectRegistry _PositionObjectRegistry;
        protected PositionObjectRegistry PositionObjectRegistry
        {
            get
            {
                if (_PositionObjectRegistry == null)
                    _PositionObjectRegistry = E.ObjectRegistries[typeof(Position)] as PositionObjectRegistry;

                return _PositionObjectRegistry;
            }
        }
        
		public override string GetSelectionPanelCategory() {
			return "Position";
		}
        
		public override bool IsEqualTo (Value value) {
			if (value.GetType() != GetType())
				return false;

            PositionObjectTypeValue otherObject = (PositionObjectTypeValue) value;

			return otherObject.ApplicableTemplates.Contains(TemplateRepresentative) && 
				   ApplicableTemplates.Contains(otherObject.TemplateRepresentative) && 
				   otherObject.Name == Name;
		}

		public PositionObjectTypeValue(Engine E, PositionType PositionType) : base(E, PositionType) {
			this.PositionType = PositionType;
			ApplicableTemplates = new List<GameObject>();
		}

		public PositionObjectTypeValue(Engine E, GameObject Template, PositionType PositionType) : base(E, PositionType) {
			TemplateRepresentative = Template;
			this.PositionType = PositionType;
			ApplicableTemplates = new List<GameObject>();
			ApplicableTemplates.Add(Template);
		}

		public PositionObjectTypeValue(Engine E, GameObject Template, PositionType PositionType, List<GameObject> ApplicableTemplates) : base(E, PositionType) {
			TemplateRepresentative = Template;
			this.ApplicableTemplates = ApplicableTemplates;
			this.PositionType = PositionType;
		}
		 
		public readonly PositionType PositionType;
		public GameObject TemplateRepresentative;
		public readonly List<GameObject> ApplicableTemplates;

        public override List<ObjectValue> GetAllInstances() {
            return PositionObjectRegistry.GenerateListOfNewBaseObjectValuesByType((PositionType) TypeInstance);
        }

        public override string GetDescription() {
			return "The type of a position. In this case " + PositionType;
		}

        public override string ToString() {
            return "PositionObjectTypeValue: " + PositionType.ToString();
        }
        
    }

}