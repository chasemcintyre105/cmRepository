
namespace RuleEngine {

	public class EffectInterfaceRegistrationAnchor : Anchor {

        private EffectFactory EffectFactory;

        public EffectInterfaceRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
            EffectFactory = initialiser.GetEngine().EffectFactory;
        }

        public void RegisterEffectInterface<EffectInterface>() where EffectInterface : Effect {
            EffectFactory.RegisterEffectInterface<EffectInterface>();
        }

        public override string GetDescription() {
            return "An anchor that allows for the registration of effect interfaces.";
        }

    }

}