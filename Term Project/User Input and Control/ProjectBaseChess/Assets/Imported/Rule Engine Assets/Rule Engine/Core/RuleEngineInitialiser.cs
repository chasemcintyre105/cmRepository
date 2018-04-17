using System;
using System.Collections.Generic;

namespace RuleEngine {

    public class RuleEngineInitialiser {

        protected Engine E;

        private Dictionary<Type, IManager> additionalManagers;
        private Dictionary<Type, IManager> overridingManagers;
        private List<ManagerSettingProfile> ManagerSettingActions;

        private class ManagerSettingProfile {
            public Action<IManager> setter;
            public Type managerType;
        }

        public HookAndAnchorFramework HooksAndAnchors { get; private set; }

        public RuleEngineInitialiser() {
            additionalManagers = new Dictionary<Type, IManager>();
            overridingManagers = new Dictionary<Type, IManager>();
            ManagerSettingActions = new List<ManagerSettingProfile>();

            HooksAndAnchors = new HookAndAnchorFramework();

            HooksAndAnchors.RegisterAnchor(new ManagerRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new ManagerConfigurationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new RuleExecutorRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new RuleExecutorFunctionRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new EffectInterfaceRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new EffectImplementationRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new RuleSorterRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new RuleVariableIteratorRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new RuleTypeRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new PlayerRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new ObjectTypeRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new ObjectRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new VariableRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new RuleRegistrationAnchor(this));
            HooksAndAnchors.RegisterAnchor(new RuleExecutionAnchor(this));

        }
        
        public void RegisterManager(IManager manager, Action<IManager> callback = null) {
            Assert.False("Manager registration is still allowed", additionalManagers == null);
            additionalManagers.Add(manager.GetType(), manager);

            if (callback != null)
                ManagerSettingActions.Add(new ManagerSettingProfile() {
                    setter = callback,
                    managerType = manager.GetType()
                });

        }

        public void OverrideManager<M>(IManager manager, Action<IManager> callback = null) where M : IManager {
            Assert.False("Manager registration is still allowed", additionalManagers == null);
            Assert.True("Manager must be of type M to override", manager is M);
            overridingManagers.Add(typeof(M), manager);

            if (!additionalManagers.ContainsKey(manager.GetType()))
                additionalManagers.Add(manager.GetType(), manager);

            if (callback != null)
                ManagerSettingActions.Add(new ManagerSettingProfile() {
                    setter = callback,
                    managerType = manager.GetType()
                });

        }

        public void Preinit(IController[] controllers, RuleEngineAddon[] addons) {

            // Create the engine instance
            RegisterManager(new ExecutionManager());
            RegisterManager(new ModificationManager());
            RegisterManager(new RuleManager());
            RegisterManager(new VariableManager());
            HooksAndAnchors.Process<ManagerRegistrationAnchor>();

            // Override managers
            foreach (Type overridingManager in overridingManagers.Keys) {

                if (!additionalManagers.ContainsKey(overridingManager))
                    throw new Exception("No manager was registered that could be overridden by " + overridingManager);

                additionalManagers[overridingManager] = overridingManagers[overridingManager];
                
            }

            E = new Engine(controllers, addons, additionalManagers);
            additionalManagers = null; // Finish the manager addon process

            // Give the correct manager instances back to the addons (taking into account overridden managers)
            foreach (ManagerSettingProfile profile in ManagerSettingActions) {
                profile.setter.Invoke(E.GetManager(profile.managerType));
            }

        }

        public void Init(Action callback) {

			SelectiveDebug.StartTimer("Initialiser");

            // This is not too late this the first anchor in Preinit doesn't use the Init method
            HooksAndAnchors.InitAllAnchors();

            // Register the basic sequential rule sorter before registering extras
            E.RuleManager.RegisterRuleSorter("Sequential", SequentialSorter);
            HooksAndAnchors.Process<RuleSorterRegistrationAnchor>();

            // Register the basic rule variable iterators before registering extras
            E.RuleManager.RegisterRuleVariableIterator("NoChange", NoChangeVariableIterator);
            E.RuleManager.RegisterRuleVariableIterator("ForEachObject", ForEachObjectVariableIterator);
            HooksAndAnchors.Process<RuleVariableIteratorRegistrationAnchor>();

            HooksAndAnchors.Process<RuleTypeRegistrationAnchor>();

            HooksAndAnchors.Process<ManagerConfigurationAnchor>();

            // Register the RuleVerifyingRuleExecutor, then pass off to the addons to register their rule executors and finally finish the rule executor registration process
            E.ExecutionManager.RegisterRuleExecutorType<RuleVerifyingExecutor>();
            HooksAndAnchors.Process<RuleExecutorRegistrationAnchor>();
            E.ExecutionManager.FinaliseRuleExecutorRegistration();

            // Register RuleVerifyingExecutor visit functions and then pass off to the addons to register their visit functions
            E.ExecutionManager.RegisterVisitFunction<RuleComponent, RuleVerifyingExecutor>(RuleVerifyingExecutor.DefaultVisit);
            E.ExecutionManager.RegisterVisitFunction<Value, RuleVerifyingExecutor>(RuleVerifyingExecutor.VisitNonNullValue);
            E.ExecutionManager.RegisterVisitFunction<NullValue, RuleVerifyingExecutor>(RuleVerifyingExecutor.VisitNullValue);
            E.ExecutionManager.RegisterVisitFunction<VoidStatement, RuleVerifyingExecutor>(RuleVerifyingExecutor.VisitVoid);
            HooksAndAnchors.Process<RuleExecutorFunctionRegistrationAnchor>();

            // Register a default visit function for each executor which throws an exception in case no visit functions are registered and the rules are only executed in compiled mode
            foreach (Type ExecutorType in E.ExecutionManager.RuleExecutorTypes) {
                if (ExecutorType != typeof(RuleVerifyingExecutor)) {
                    E.ExecutionManager.RegisterVisitFunction(typeof(RuleComponent), ExecutorType, delegate (RuleExecutor RuleExecutor, RuleComponent obj) { Assert.Never("Unimplemented visit function for " + obj.GetType().Name + " in " + RuleExecutor.GetType().Name); });
                }
            }

            HooksAndAnchors.Process<RuleExecutorFunctionRegistrationAnchor>();

            // Register engine effects, allow addons to register effects, and finalise registration
            E.EffectFactory.RegisterEffectInterface<IDisplayMessageEffect>();
            E.EffectFactory.RegisterEffectInterface<IStartedLoadingEffect>();
            E.EffectFactory.RegisterEffectInterface<IFinishedLoadingEffect>();
            E.EffectFactory.RegisterEffectInterface<IRuleExceptionEffect>();
            HooksAndAnchors.Process<EffectInterfaceRegistrationAnchor>();
            HooksAndAnchors.Process<EffectImplementationRegistrationAnchor>();
            E.EffectFactory.FinaliseRegistration();

            HooksAndAnchors.Process<PlayerRegistrationAnchor>();

            // Register basic number types before allowing registration of others
            E.IntegerObjectRegistry.RegisterObjectType_TS(E.IntegerObjectRegistry.IntegerType);
            E.FloatObjectRegistry.RegisterObjectType_TS(E.FloatObjectRegistry.FloatType);
            HooksAndAnchors.Process<ObjectTypeRegistrationAnchor>();

            HooksAndAnchors.Process<ObjectRegistrationAnchor>();

            HooksAndAnchors.Process<VariableRegistrationAnchor>();

            // Register, verify and compile rules
            HooksAndAnchors.Process<RuleRegistrationAnchor>();
            E.ExecutionManager.VerifyRulesAsynchronously(delegate (RuleVerifyingExecutor RuleExecutor) {

                E.ExecutionManager.CompileRulesAsynchronously(delegate {

                    // Let the addons execute any rules that need executing
                    HooksAndAnchors.Process<RuleExecutionAnchor>();

                    callback(); // Pass control back to the RuleEngineController
                });

            });

        }

        // Default sorter function
        private IEnumerable<int> SequentialSorter(RuleExecutor RuleExecutor, RuleType RuleType) {
            for (int index = 0; index < E.RuleManager.RuleTypeToList[RuleType].Count; index++)
                yield return index;
        }

        // Default variable iterator functions
        private IEnumerable<bool> NoChangeVariableIterator(RuleExecutor RuleExecutor, Rule rule) {
            yield return true; // Run just once
        }

        private IEnumerable<bool> ForEachObjectVariableIterator(RuleExecutor RuleExecutor, Rule rule) {

            int VariableCount = rule.Variables.Count;
            int[] counters = ArrayIteration.NewZeroedIntArray(VariableCount);
            int[] limits = ArrayIteration.NewZeroedIntArray(VariableCount);
            List<List<ObjectValue>> objectLists = new List<List<ObjectValue>>();

            // Set limits
            bool AtLeastOneCountIsZero = false;
            for (int i = 0; i < VariableCount; i++) {
                objectLists.Add(rule.Variables[i].VariableType.GetAllInstances());
                limits[i] = objectLists[i].Count;

                if (limits[i] == 0)
                    AtLeastOneCountIsZero = true;

            }

            // If there are variables in this rule, iterate through all combinations of each instance of each variable type
            if (VariableCount > 0 && !AtLeastOneCountIsZero) {

                do {

                    // Stop the process if the flag is set
                    if (RuleExecutor.StopExecutingRules)
                        break;

                    // Assign the next combination of values to the enumerable variables of this rule
                    for (int i = 0; i < VariableCount; i++) {

                        // If the variable is a unit and the unit has been removed, skip this instance of this type
                        if (counters[i] >= objectLists[i].Count)
                            break;

                        rule.Variables[i].VariableValue = objectLists[i][counters[i]];
                        SelectiveDebug.LogVariable("Variable " + (i + 1) + " (" + rule.Variables[i].VariableName + ") takes value " + rule.Variables[i].VariableValue);
                    }

                    yield return true;

                } while (!ArrayIteration.IncrementCombinatorialArray(counters, limits));

            } else {
                yield return true;
            }

        }

        public Engine GetEngine() {
			return E;
		}

	}

}
