using UnityEngine;

namespace RuleEngine {

	public abstract class IController : MonoBehaviour {

        protected Engine E;

        public void SetEnabled(bool enabled) {
            this.enabled = enabled;
        }

        public void SetEngine(Engine E) {
            this.E = E;
        }

        public abstract void Preinit();
        public abstract void Init();

    }

}
