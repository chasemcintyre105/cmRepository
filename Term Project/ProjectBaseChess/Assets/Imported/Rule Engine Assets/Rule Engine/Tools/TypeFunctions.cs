using System;
using System.Reflection;

namespace RuleEngine {

    public class TypeFunctions {

		public static bool IsSameOrSubclassOf(Type potentialDescendant, Type potentialBase) {
			return potentialDescendant == potentialBase || potentialDescendant.IsSubclassOf(potentialBase);
		}

		public static void CheckAllFieldsNotNull(object obj) {
			if (obj == null)
				throw new Exception("Object not yet initialised by the editor");
            foreach (FieldInfo p in obj.GetType().GetFields()) {
                var q = p.GetValue(obj);
                if (q == null || q.Equals("null"))
                    throw new Exception("Field " + p.Name + " of " + obj.GetType().Name + " is null.");
            }
		}
        
	}

}