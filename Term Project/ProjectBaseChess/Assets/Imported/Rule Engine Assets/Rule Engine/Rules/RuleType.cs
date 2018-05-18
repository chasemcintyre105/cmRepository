
namespace RuleEngine {

    public class RuleType {

        public string ID { get; private set; }

        public RuleType(string ID) {
            this.ID = ID;
        }

        public bool Is(RuleType type) {
            return type.ID == ID;
        }

        public override string ToString() {
            return ID;
        }

    }

}
