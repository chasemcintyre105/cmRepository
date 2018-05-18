
namespace RuleEngine {

	public class EffectImplementationRegistrationAnchor : Anchor {

        private EffectFactory EffectFactory;

        public EffectImplementationRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
            EffectFactory = initialiser.GetEngine().EffectFactory;
        }

        public void RegisterEffectImplementation<EffectInterface, EffectImplementation>() where EffectInterface : Effect where EffectImplementation : EffectInterface {
            EffectFactory.RegisterEffectImplementation<EffectInterface, EffectImplementation>();
        }

        public void OverrideEffectImplementation<EffectInterface, EffectImplementation>() where EffectInterface : Effect where EffectImplementation : EffectInterface {
            EffectFactory.OverrideEffectImplementation<EffectInterface, EffectImplementation>();
        }

        public override string GetDescription() {
            return "An anchor that allows for the registration of effect implementations.";
        }

    }

}