using System.Collections.Generic;

namespace RuleEngine {

    public abstract class ObjectTypeValue : GameObjectValue {
        
		public ObjectTypeValue(Engine E, IObjectType TypeInstance) : base(E) {
			Name = TypeInstance.GetID();
            this.TypeInstance = TypeInstance;
		}
		 
		public readonly string Name;
        public readonly IObjectType TypeInstance;

        public abstract List<ObjectValue> GetAllInstances();

    }

}