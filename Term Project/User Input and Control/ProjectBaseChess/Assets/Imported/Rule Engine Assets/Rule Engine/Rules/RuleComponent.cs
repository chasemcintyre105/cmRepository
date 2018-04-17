using System;

namespace RuleEngine {
	
	public abstract class RuleComponent {

        public enum RuleComponentEditability {
            Not_Editable,
            Not_Chosen,
            Editable
        }

		protected Engine E;
        
		public abstract Type GetReturnType();
		public abstract void DefineArguments();
		public abstract string GetSelectionPanelCategory();
		public abstract string GetDescription();
        public abstract CompiledRuleExecutable NewCRE();

        // Replacement RuleExecutor function
        public readonly ExecutionManager.VisitAction[] accept;

        public ArgumentList ArgumentList { get; private set; }

        public RuleComponentEditability Editability;

		public RuleComponent(Engine E) {
			this.E = E;
            Editability = RuleComponentEditability.Not_Chosen;
            ArgumentList = new ArgumentList(this, 0);
            DefineArguments();
            
            accept = E.ExecutionManager.GetVisitActions(GetType());

            Assert.NotNull("There are visit actions", accept);

        }
        
		// Type functions
		public bool Is<T>() {
			return (this is T);
		}
		
		public T As<T>() where T : RuleComponent {
			return (this as T);
		}

		public void FillArgumentsWithNullValues() {
			for (int i = 0; i < ArgumentList.argsByOrder.Count; i++) {
				ArgumentList.SetArgument(i, NullValue.Instance.NewRef());
			}
		}
		
		public void FillArgumentsWithVoid() {
			for (int i = 0; i < ArgumentList.argsByOrder.Count; i++) {
				ArgumentList.SetArgument(i, VoidStatement.Instance.NewRef());
			}
		}

		public RuleComponentReference NewRef() {
			return new RuleComponentReference(this);
		}

	}
	
}