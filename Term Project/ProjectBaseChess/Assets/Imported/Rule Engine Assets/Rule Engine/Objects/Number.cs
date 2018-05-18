using System;

namespace RuleEngine {

    public abstract class Number : IObject {

        public abstract object GetValue();

        private int ID;
        protected IObjectType type;

        public Number(int ID, IObjectType type) {
            this.ID = ID;
            this.type = type;
        }

        public int GetID() {
            return ID;
        }

        public IObjectType GetObjectType() {
            return type;
        }

    }

}