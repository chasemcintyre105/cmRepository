
namespace RuleEngine {

	public class VariableRegistrationAnchor : Anchor {

        private VariableManager VariableManager;

        public VariableRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
            VariableManager = initialiser.GetEngine().VariableManager;
        }
        
        public void NewVariable(string name, ObjectTypeValue type) {
            VariableManager.NewVariable_TS(name, type);
        }

        public override string GetDescription() {
            return "An anchor that allows for the registration of variables for use in rules.";
        }

    }

}