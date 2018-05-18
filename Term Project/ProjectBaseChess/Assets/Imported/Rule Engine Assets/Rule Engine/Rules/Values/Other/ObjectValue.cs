namespace RuleEngine {

    public abstract class ObjectValue : GameObjectValue {

		public ObjectValue(Engine E, ObjectTypeValue TypeValue, IObject Instance) : base(E) {
			this.TypeValue = TypeValue;
			this.Instance = Instance;
		}
		
		public override bool IsEqualTo (Value value) {
			if (value.GetType() != GetType())
				return false;
			
			ObjectValue otherObject = ((ObjectValue) value);
			
			return otherObject.TypeValue.IsEqualTo(TypeValue) && 
				otherObject.Instance.Equals(Instance);
		}

		public readonly ObjectTypeValue TypeValue;
		public readonly IObject Instance;

	}

}