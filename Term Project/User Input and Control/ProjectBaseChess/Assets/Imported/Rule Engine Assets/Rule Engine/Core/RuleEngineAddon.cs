using System;
using UnityEngine;

namespace RuleEngine {

	public abstract class RuleEngineAddon : MonoBehaviour {

        public abstract void Preinit();
        public abstract void Init();
        public abstract void RegisterAnchors();
        public abstract void RegisterHooks();

        protected RuleEngineInitialiser initialiser;
        protected Engine E;

        public void SetInitialiser(RuleEngineInitialiser initialiser) {
            this.initialiser = initialiser;
        }

        public void SetEngine(Engine E) {
            this.E = E;
        }

        protected void RegisterAnchor<A>(A anchor) where A : Anchor {
            initialiser.HooksAndAnchors.RegisterAnchor<A>(anchor);
        }

        protected void RegisterHookWithAnchor<A>(Action<A> action) where A : Anchor {
            initialiser.HooksAndAnchors.RegisterHook(new Hook<A>(action));
        }

        protected void ProcessAnchor<A>() where A : Anchor {
            initialiser.HooksAndAnchors.Process<A>();
        }

    }

}
