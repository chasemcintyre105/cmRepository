using System;
using System.Collections.Generic;

namespace RuleEngine {
	
	public class Argument {

        public string name;
        public int index = -1;

        public RuleComponent parentObject; // Object that contains the argument list that contains the argument
		public RuleComponentReference reference;

		public List<Type> AcceptableArgumentTypes;
		public string InterfaceDescription = "";
		public bool TransformObjectTypeIntoVariable = true;

		public Argument() {
			AcceptableArgumentTypes = new List<Type>();
		}

	}
	
}