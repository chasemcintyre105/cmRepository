using System;
using System.Collections.Generic;

namespace RuleEngine {

    /*
     * Provides a way to execute a set of rules more efficiently by building a structure of class instances
     * Each instance has the appropriate code for its rule and the instance are interlinked according to 
     * the rules they are derived from. When the root instance is executed it executed all linked classes appropriatly
     */
    public abstract class CompiledRuleExecutable {

        public abstract bool IsValueType(Type type);
        public abstract O CalculateValue<O>(RuleExecutor RuleExecutor) where O : RuleComponent;
        
        protected Engine E;
        protected RuleComponent associatedObj;
        protected Dictionary<string, CREArgument> argsByName;
        protected List<CREArgument> argsByOrder;
        
        public CompiledRuleExecutable(Engine E, RuleComponent associatedObj) {
            this.E = E;
            this.associatedObj = associatedObj;
            argsByName = new Dictionary<string, CREArgument>();
            argsByOrder = new List<CREArgument>();

            List<Argument> orderedArguments = associatedObj.ArgumentList.argsByOrder;

            for (int index = 0; index < orderedArguments.Count; index++) {
                Argument a = orderedArguments[index];
                RuleComponent obj = a.reference.Instance();
                CREArgument newCREArg = new CREArgument(a.name, obj.NewCRE());

                Assert.NotNull("A name must be given", a.name);

                argsByName.Add(a.name, newCREArg);
                argsByOrder.Add(newCREArg);

                // Set the same parent and index data into the CRE argument to allow for the object substitutions
                newCREArg.parent = associatedObj;
                newCREArg.index = index;

            }

        }

    }

}
