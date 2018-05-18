using RuleEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewObjectType : IObjectType {

        private string Name;

        public NewObjectType(string Name) {
            this.Name = Name;
        }

        public string GetID() {
            return Name;
        }

    }

}

