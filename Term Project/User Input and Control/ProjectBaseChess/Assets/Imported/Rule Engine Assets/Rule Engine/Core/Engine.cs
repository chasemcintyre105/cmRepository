using System;
using System.Collections.Generic;

namespace RuleEngine {
	
	public sealed class Engine {
        
		// Managers
        public RuleManager RuleManager { get; private set; }
		public ExecutionManager ExecutionManager { get; private set; }
		public ModificationManager ModificationManager { get; private set; }
        public VariableManager VariableManager { get; private set; }

        private Dictionary<Type, IManager> managers;

        // Controllers
        public EffectController EffectController { get; private set; }
        public ThreadController ThreadController { get; private set; }

        private Dictionary<Type, IController> controllers;

        // Addons
        private List<string> addons;

		// Implementation specific objects
		public EffectFactory EffectFactory;

        // Object registries
        public IntegerObjectRegistry IntegerObjectRegistry { get; private set; }
        public FloatObjectRegistry FloatObjectRegistry { get; private set; }
        public Dictionary<Type, ObjectRegistry> ObjectRegistries;

        // Custom objects and settings
        private readonly object _customSettingsLock = new object();
        private Dictionary<string, object> customSettings;

        public Engine(IController[] controllers, RuleEngineAddon[] addons, Dictionary<Type, IManager> addonManagers) {

            EffectFactory = new EffectFactory();

            // Set up addon class name list
            this.addons = new List<string>();
            foreach (RuleEngineAddon addon in addons) {
                this.addons.Add(addon.GetType().Name);  
            }

            // Set up controllers dictionary
            this.controllers = new Dictionary<Type, IController>();
			foreach (IController c in controllers) {
				this.controllers.Add(c.GetType(), c);

                if (c is EffectController)
                    EffectController = c as EffectController;
                else if (c is ThreadController)
                    ThreadController = c as ThreadController;

            }
            Assert.NotNull("EffectController is enabled", EffectController);
            Assert.NotNull("ThreadController is enabled", ThreadController);

            // Initialise managers
            managers = new Dictionary<Type, IManager>();
            foreach (Type type in addonManagers.Keys) {
                IManager manager = addonManagers[type];

                if (managers.ContainsKey(type))
                    throw new Exception("Manager is already registered: " + type.Name);

                managers.Add(type, manager);
                manager.SetEngine(this);
            }

            // Register essential managers
            ExecutionManager = GetManager<ExecutionManager>();
            ModificationManager = GetManager<ModificationManager>();
            RuleManager = GetManager<RuleManager>();
            VariableManager = GetManager<VariableManager>();
            
            // Initialise the object registries dictionary
            ObjectRegistries = new Dictionary<Type, ObjectRegistry>();

            // Initialise the custom settings dictionary
            customSettings = new Dictionary<string, object>();

        }

        // Get controller function
		public C GetController<C>() where C : IController {
			IController controller;
            controllers.TryGetValue(typeof(C), out controller);
			return (C) controller;
		}

        // Get manager(s) functions
        public M GetManager<M>() where M : IManager {
            IManager manager;
            managers.TryGetValue(typeof(M), out manager);
            return (M) manager;
        }

        public IManager GetManager(Type managerType) {
            IManager manager;
            managers.TryGetValue(managerType, out manager);
            return manager;
        }

        public IEnumerable<IManager> Managers {
            get {
                return managers.Values;
            }
        }

        // Addon functions
        public bool HasAddon(string className) {
            return addons.Contains(className);
        }

        public bool HasAddon<A>() where A : RuleEngineAddon {
            return addons.Contains(typeof(A).Name);
        }

        public bool HasController<C>() where C : IController {
            return controllers.ContainsKey(typeof(C));
        }
        
        // Object registries
        public SpecificObjectRegistry<O, OT, OV, OTV> RegisterNewObjectRegistry<O, OT, OV, OTV>(SpecificObjectRegistry<O, OT, OV, OTV>.ObjectValueCreator NewObjectValue, 
                                                                                                SpecificObjectRegistry<O, OT, OV, OTV>.ObjectTypeValueCreator NewObjectTypeValue) 
                                                                                                        where O : IObject
                                                                                                        where OT : IObjectType
                                                                                                        where OV : ObjectValue
                                                                                                        where OTV : ObjectTypeValue {

            SpecificObjectRegistry<O, OT, OV, OTV> registry = new SpecificObjectRegistry<O, OT, OV, OTV>(this, NewObjectValue, NewObjectTypeValue);
            ObjectRegistries.Add(typeof(O), registry);

            CheckForInBuiltObjectRegistry(registry);

            return registry;
        }

        public void RegisterNewObjectRegistry(ObjectRegistry ObjectRegistry) {
            ObjectRegistries.Add(ObjectRegistry.GetRegistryObjectType(), ObjectRegistry);
            CheckForInBuiltObjectRegistry(ObjectRegistry);
        }

        private void CheckForInBuiltObjectRegistry(ObjectRegistry ObjectRegistry) {
            Type registryObjectType = ObjectRegistry.GetRegistryObjectType();
            if (registryObjectType == typeof(Integer)) {
                IntegerObjectRegistry = ObjectRegistry as IntegerObjectRegistry;
            } else if (registryObjectType == typeof(Float)) {
                FloatObjectRegistry = ObjectRegistry as FloatObjectRegistry;
            }
        }

        public OR GetObjectRegistry<Obj, OR>() where OR : ObjectRegistry {
            ObjectRegistry registry = null;

            if (!ObjectRegistries.TryGetValue(typeof(Obj), out registry))
                throw new Exception("Object registry with base object type " + typeof(Obj).Name + " not registered");

            if (!(registry is OR))
                throw new Exception("Registry is not of the expected type: " + typeof(OR).Name + " (Found " + registry.GetType().Name + ")");

            return registry as OR;
        }

        public ObjectRegistry GetObjectRegistry<Obj>() {
            ObjectRegistry registry = null;

            if (!ObjectRegistries.TryGetValue(typeof(Obj), out registry))
                throw new Exception("Object registry with base object type " + typeof(Obj).Name + " not registered");

            return registry;
        }

        // Custom settings functions
        public void Set_TS(string SettingID, object obj) {
            lock (_customSettingsLock) { 
                Assert.False("Setting does not yet exist", customSettings.ContainsKey(SettingID));
                customSettings.Add(SettingID, obj);
            }
        }

        public S Get_TS<S>(string SettingID) {
            object obj;
            lock (_customSettingsLock) {
                Assert.True("Setting exists and is the right type", customSettings.TryGetValue(SettingID, out obj) && obj is S);
                return (S) obj;
            }
        }

        public bool TryGet_TS<S>(string SettingID, out S value) {
            object obj;
            lock (_customSettingsLock) {
                if (customSettings.TryGetValue(SettingID, out obj) && obj is S) {
                    value = (S) obj;
                    return true;
                } else {
                    value = default(S);
                    return false;
                }
            }
        }

        public bool Has_TS(string SettingID) {
            lock (_customSettingsLock) {
                return customSettings.ContainsKey(SettingID);
            }
        }

        public void Delete_TS(string SettingID) {
            lock (_customSettingsLock) {
                Assert.True("The settings id exists", customSettings.ContainsKey(SettingID));
                customSettings.Remove(SettingID);
            }
        }

    }

}
