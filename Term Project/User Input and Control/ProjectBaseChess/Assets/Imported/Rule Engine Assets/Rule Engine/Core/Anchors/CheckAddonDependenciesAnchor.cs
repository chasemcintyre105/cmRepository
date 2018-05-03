using System;

namespace RuleEngine {

	public class CheckDependenciesAnchor : Anchor {

        public CheckDependenciesAnchor(RuleEngineInitialiser initialiser) : base(initialiser) {
        }

        public override void Init() {
        }

        public void RequireAddon<A>() where A : RuleEngineAddon {
            if (!initialiser.GetEngine().HasAddon<A>())
                throw new Exception("Addon required another addon that is not present: " + typeof(A).Name);
        }

        public C RequireController<C>() where C : IController {
            C controller = initialiser.GetEngine().GetController<C>();

            if (controller == null)
                throw new Exception("Addon required controller that is not present: " + typeof(C).Name);

            return controller;
        }

        public override string GetDescription() {
            return "An anchor that allows dependencies on other addons and controllers to be checked.";
        }
        
    }

}