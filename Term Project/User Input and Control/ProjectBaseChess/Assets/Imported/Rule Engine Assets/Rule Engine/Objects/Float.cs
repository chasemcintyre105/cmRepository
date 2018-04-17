using System;

namespace RuleEngine {

    public class Float : Number {
        
        private readonly float value;

        public Float(int ID, float value, IObjectType type) : base (ID, type) {
            this.value = value;
        }

        public override object GetValue() {
            return value;
        }

        public virtual float GetFloatValue() {
            return value;
        }

        public virtual bool IsEqualTo(Float number) {
            return number.value == value;
        }

        public override bool Equals(object obj) {

            if (obj is Float)
                return false;

            return IsEqualTo(obj as Float);
        }

        public override int GetHashCode() {
            return (int) Math.Round((decimal) (521 * value));
        }

    }

}