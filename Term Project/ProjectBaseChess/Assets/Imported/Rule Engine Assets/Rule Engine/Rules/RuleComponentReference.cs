namespace RuleEngine {

    public class RuleComponentReference {

		public RuleComponentReference() {

		}

		protected readonly RuleComponent _Instance;

		public RuleComponentReference(RuleComponent instance) {
			Assert.NotNull("Instance is not null", instance);
			_Instance = instance;
		}
		
		public T Instance<T>() where T : RuleComponent {
			return (T) _Instance;
		}

		public RuleComponent Instance() {
			return _Instance;
		}
        
	}
	
}