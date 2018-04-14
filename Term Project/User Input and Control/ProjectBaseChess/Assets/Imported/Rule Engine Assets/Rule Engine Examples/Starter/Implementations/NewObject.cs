using RuleEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewObject : IObject {

        public readonly int ID;
        public readonly IObjectType Type;

        public NewObject(int ID, IObjectType Type) {
            this.ID = ID;
            this.Type = Type;
        }

        public int GetID() {
            return ID;
        }

        public IObjectType GetObjectType() {
            return Type;
        }

        public override string ToString() {
            return "NewObject with ID " + ID;
        }

    }

}

