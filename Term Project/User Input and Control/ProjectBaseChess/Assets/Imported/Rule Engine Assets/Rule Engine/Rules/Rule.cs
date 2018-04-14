using System;
using System.Collections.Generic;

namespace RuleEngine {

    public class Rule {

		public Rule(Engine E) {
			Block = new Block(E);
			Block.AddStatement(VoidStatement.Instance.NewRef());
			Locked = false;
            Irremovalable = false;
            CollisionOnly = false;
            SomeElementsEditable = false;
            PossibleActionDependent = false;
			Variables = new List<Variable>();
			VariablesByType = new Dictionary<Type, List<Variable>>();
		}

        public string Name = null;
        public string UID = null;
        public bool Locked;
        public bool Irremovalable;
        public bool CollisionOnly;
        public bool SomeElementsEditable;
        public bool PossibleActionDependent;

        // The rule contents
        public Block Block;
        public CompiledRuleExecutable CompiledRule;

        // A list of variables that represent object types that appear in this rule.
        // These variable are assumed to be scoped to only this rule.
        // It is expected that any RuleExecutor that visits this rule with the intention of execution will run
        // once for each combination of instances of each type that is represented by the variables in this list.
        // This list is maintained by the rule renderer RuleExecutor.
        public List<Variable> Variables;
		public Dictionary<Type, List<Variable>> VariablesByType;

        public override string ToString() {
            return Name;
        }

    }

}
