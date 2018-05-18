using System.Collections.Generic;

namespace RuleEngine {

    public class VariableManager : IManager {
        
        protected readonly object _variableLock = new object();

        protected Dictionary<string, Variable> variables;
        
        public VariableManager() {
            variables = new Dictionary<string, Variable>();
        }
        
        public override void Preinit() {
        }

        public override void Init() {
            
        }

        public virtual void NewVariable_TS(string name, ObjectTypeValue type) {
            lock (_variableLock) {
                Assert.False("Variable does not already exist: " + name, variables.ContainsKey(name));
                variables.Add(name, new Variable(E, name, type, null));
            }
        }

        public virtual void DeleteVariable_TS(string name) {
            lock (_variableLock) {
                Assert.True("Variable exists: " + name, variables.ContainsKey(name));
                variables.Remove(name);
            }
        }

        public virtual void SetVariable_TS(string name, Value value) {
            lock (_variableLock) {
                Variable variable;
                Assert.True("Variable exists: " + name, variables.TryGetValue(name, out variable));
                variable.VariableValue = value;
            }
        }

        public virtual Variable GetVariable_TS(string name) {
            lock (_variableLock) {
                Variable variable;
                Assert.True("Variable exists: " + name, variables.TryGetValue(name, out variable));
                return variable;
            }
        }

    }
	
}