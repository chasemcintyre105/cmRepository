
namespace RuleEngine {

	public abstract class IManager {

        protected Engine E;

        public void SetEngine(Engine E) {
            this.E = E;
        }

        public abstract void Preinit();
        public abstract void Init();

    }

}