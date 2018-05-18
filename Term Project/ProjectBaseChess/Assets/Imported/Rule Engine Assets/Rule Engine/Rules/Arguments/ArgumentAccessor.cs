using System;

namespace RuleEngine {

    public class ArgumentAccessor {

		public RuleComponent obj;
		public int index;

		public ArgumentAccessor(RuleComponent obj, int index) {
			this.obj = obj;
			Assert.True("Index is valid for this object", index >= 0 && index < obj.ArgumentList.GetNumberOfArguments());
			this.index = index;
		}

		public ArgumentAccessor(RuleComponentReference reference, int index) {
			this.obj = reference.Instance();
			Assert.True("Index is valid for this object", index >= 0 && index < obj.ArgumentList.GetNumberOfArguments());
			this.index = index;
		}

		public Argument GetArgumentObject() {
			return obj.ArgumentList.argsByOrder[index];
		}

		public RuleComponentReference Argument {
			get {
				return obj.ArgumentList.GetArgument(index);
			}
			set {
				obj.ArgumentList.SetArgument(index, value);
			}
		}

		public bool CanBeSetAsArgument(RuleComponentReference reference) {
			return this.obj.ArgumentList.CanBeSetAsArgument(index, reference);
		}
		
		public bool CanBeSetAsArgument(RuleComponent obj) {
			return this.obj.ArgumentList.CanBeSetAsArgument(index, obj);
		}

		public bool ContainsAcceptableType(Type type) {
			return GetArgumentObject().AcceptableArgumentTypes.Contains(type);
		}

	}
	
}