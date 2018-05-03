using System;

namespace RuleEngine {

    public class NumberType : IObjectType {

        private string ID;

        public NumberType(string ID) {
            this.ID = ID;
        }

        public string GetID() {
            return ID;
        }

    }

}