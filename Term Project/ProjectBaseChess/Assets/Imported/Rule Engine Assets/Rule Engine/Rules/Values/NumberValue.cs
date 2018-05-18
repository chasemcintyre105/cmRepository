namespace RuleEngine {

    public class NumberValue : ObjectValue {
        
		public override string GetSelectionPanelCategory() {
			return "Number";
		}
        
		public override bool IsEqualTo (Value v) {

			if (v.GetType() != typeof(NumberValue))
				return false;

			return ((NumberValue) v).numberValue == numberValue;
		}

		public readonly Number numberValue;

		public NumberValue(Engine E, NumberTypeValue numberType, Number numberValue) : base(E, numberType, numberValue) { 
			this.numberValue = numberValue;
		}
		
		public override string GetDescription() {
			return "The number " + Instance;
		}

        public override string ToString() {
            return Instance.ToString();
        }

    }


}