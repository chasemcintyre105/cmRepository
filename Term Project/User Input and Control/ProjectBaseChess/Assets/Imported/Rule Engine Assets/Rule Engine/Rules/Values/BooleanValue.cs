using System;

namespace RuleEngine {

    public class BooleanValue : Value {
        
		public override string GetSelectionPanelCategory() {
			return "Boolean";
		}
        
		public override bool IsEqualTo (Value v) {

			if (v.GetType() != typeof(BooleanValue))
				return false;

			return ((BooleanValue) v).value == value;
		}

		public readonly bool value;

		public BooleanValue(Engine E, bool value) : base(E) { 
			this.value = value; 
		}
		
		public override string GetDescription() {
			return "The value true or false. Used to determine the application of rules in If statements and much more.";
		}

        public override string ToString() {
            return value.ToString();
        }

    }

}