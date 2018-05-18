using System;

namespace RuleEngine {

    public class Integer : Number {
        
        private readonly int value;
        
        public Integer(int value, IObjectType type) : base(value, type) {
            this.value = value;
        }

        public override object GetValue() {
            return value;
        }

        public virtual float GetIntegerValue() {
            return value;
        }

        public virtual bool IsEqualTo(Integer number) {
            return number.value == value;
        }

        public override bool Equals(object obj) {

            if (obj is Integer)
                return false;

            return IsEqualTo(obj as Integer);
        }

        public override int GetHashCode() {
            return (int) Math.Round((decimal) (521 * value));
        }

    }

}