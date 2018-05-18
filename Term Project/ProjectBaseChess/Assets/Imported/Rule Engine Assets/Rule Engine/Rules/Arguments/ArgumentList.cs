using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngine {

    public class ArgumentList {
		
		public RuleComponent parent;
        public Dictionary<string, Argument> argsByName;
        public List<Argument> argsByOrder;

		public ArgumentList(RuleComponent parent, int count) {
			Assert.NotNull("Parent", parent);
			this.parent = parent;
            argsByName = new Dictionary<string, Argument>();
            argsByOrder = new List<Argument>();
			for (int i = 0; i < count; i++)
				argsByOrder.Add(new Argument());
		}

		public RuleComponentReference GetArgument(int i) {
			Assert.True("Index within range (" + argsByOrder.Count + "): " + i, i >= 0 && i < argsByOrder.Count);
			return argsByOrder[i].reference;
		}

		public void SetArgument(int i, RuleComponent obj) {
			SetArgument(i, obj.NewRef());
		}

		public void SetArgument(int i, RuleComponentReference reference) {
			Assert.True("Index within range (" + argsByOrder.Count + "): " + i, i >= 0 && i < argsByOrder.Count);
			Assert.NotNull("args[i]", argsByOrder[i]);
			Assert.True("The object being set is an acceptable type", CanBeSetAsArgument(i, reference));

			Argument arg = argsByOrder[i];

			arg.parentObject = parent;
			arg.reference = reference; 
			
			UpdateArgumentIndexesFrom(i);
		}

		public bool CanBeSetAsArgument(int i, RuleComponentReference reference) {
			return CanBeSetAsArgument(i, reference.Instance());
		}

		public bool CanBeSetAsArgument(int i, RuleComponent obj) {

            if (obj == null)
                throw new Exception("Obj being set in argument is not null");

            // Find the relevant type of the object
            // Note that Variables automatically give the return type of their contained value
            Type typeToCheck = obj.GetReturnType();

            bool acceptable = false;
            if (typeToCheck == typeof(NullValue)) { // Null values may only occupy the place of a Value

                foreach (Type acceptableType in argsByOrder[i].AcceptableArgumentTypes)
                    acceptable = acceptable || TypeFunctions.IsSameOrSubclassOf(acceptableType, typeof(Value));

            } else if (typeToCheck == typeof(VoidStatement)) { // Void statements may only occupy the place of a Statement

                foreach (Type acceptableType in argsByOrder[i].AcceptableArgumentTypes)
                    acceptable = acceptable || TypeFunctions.IsSameOrSubclassOf(acceptableType, typeof(Statement));

            } else { // Otherwise check that the value being set is acceptable as one of the available argument type

                foreach (Type acceptableType in argsByOrder[i].AcceptableArgumentTypes)
                    acceptable = acceptable || TypeFunctions.IsSameOrSubclassOf(typeToCheck, acceptableType);

            }

            return acceptable;
		}

		public int GetNumberOfArguments() {
			return argsByOrder.Count;
		}

        public void DefineArgument(string Name, Type[] types, string description, bool TransformObjectTypeIntoVariable = true) {
            Argument newArg = new Argument() {
                index = argsByOrder.Count,
                name = Name,
                InterfaceDescription = description,
                TransformObjectTypeIntoVariable = TransformObjectTypeIntoVariable
            };
            newArg.AcceptableArgumentTypes.AddRange(types);
            argsByName.Add(Name, newArg);
            argsByOrder.Add(newArg);
        }

        public void DefineArgument(string Name, Type type, string description, bool TransformObjectTypeIntoVariable = true) {
            Argument newArg = new Argument() {
                index = argsByOrder.Count,
                name = Name,
                InterfaceDescription = description,
                TransformObjectTypeIntoVariable = TransformObjectTypeIntoVariable
            };
            newArg.AcceptableArgumentTypes.Add(type);
            argsByName.Add(Name, newArg);
            argsByOrder.Add(newArg);
        }
        
		public void UpdateArgumentIndexesFrom(int from) {
			for (int i = from; i < argsByOrder.Count; i++)
				argsByOrder[i].index = i;
		}

        public void UpdateArgumentOfBlockFrom(int from) {
            for (int i = from; i < argsByOrder.Count; i++) { 
                argsByOrder[i].index = i;
                argsByOrder[i].name = i.ToString();
            }
        }

    }

}