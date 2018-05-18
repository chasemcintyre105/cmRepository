using System;

namespace RuleEngine {

	public class ManagerRegistrationAnchor : Anchor {

        public ManagerRegistrationAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }

        public void RegisterManager(IManager manager, Action<IManager> callback = null) {
            initialiser.RegisterManager(manager, callback);
        }

        public void OverrideManager<M>(IManager manager, Action<IManager> callback = null) where M : IManager {
            initialiser.OverrideManager<M>(manager, callback);
        }

        public override string GetDescription() {
            return "An anchor that allows for the registration of new managers.";
        }

    }

}