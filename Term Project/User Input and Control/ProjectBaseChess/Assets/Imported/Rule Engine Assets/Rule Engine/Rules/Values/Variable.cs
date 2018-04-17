using System;

namespace RuleEngine {

    public class Variable : Value {
		
		public override Type GetReturnType() {
			return VariableValue.GetReturnType(); 
		}
		
		public override string GetSelectionPanelCategory() {
			return "Variables";
		}

		public Variable(Engine E, string VariableName, ObjectTypeValue VariableType, Value VariableValue) : base(E) {
			this.VariableName = VariableName;
			this.VariableValue = VariableValue;
			this.VariableType = VariableType;
		}
		
		public override void DefineArguments() {
		}
		
		public override bool IsEqualTo (Value value) {
			if (value is Variable)
				return ((Variable) value).VariableValue.IsEqualTo(VariableValue);
			
			return value.IsEqualTo(VariableValue);
		}

		public readonly string VariableName;
		public ObjectTypeValue VariableType;

		private Value _VariableValue;
		public Value VariableValue {
			get {
				return _VariableValue;
			}
			set {
				if (value == null)
					_VariableValue = NullValue.Instance; 
				else {
					_VariableValue = value;
				}
			}
		}
		
		public override string GetDescription() {
			return _VariableValue.GetDescription();
		}

        public override string ToString() {
            return VariableName;
        }
        
    }

}