using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngine {

    public class RuleEngineController : MonoBehaviour {

        // Global references to important objects
        public static Engine E { get; private set; }
        public static RuleEngineController I;
        
        // Private objects
        private RuleEngineInitialiser initialiser;
        private IController[] controllers;
        private RuleEngineAddon[] addons;
        
		void Awake() {

            // Set static reference
            I = this;

            // Reset Engine reference
            E = null;

            RuleEngineController[] AbstractionControllers = FindObjectsOfType<RuleEngineController>();
            Assert.True("There is only one AbstractionController in this scene", AbstractionControllers.Length == 1);

            SelectiveDebug.LogInit("Awake");
			SelectiveDebug.StartTimer("TotalSceneLoad");

            // Run all preload components
            SelectiveDebug.LogInit("Disable all controllers temporarily");
            IPreloadComponent[] preloads = GetComponentsInChildren<IPreloadComponent>();
            foreach (IPreloadComponent preload in preloads)
                preload.Init();
            
            // Get all available controllers and turn them off for now
            SelectiveDebug.LogInit("Disable all controllers temporarily");
            controllers = GetComponents<IController>();
			foreach (IController controller in controllers)
				controller.SetEnabled(false);
            
            initialiser = new RuleEngineInitialiser();

            // Register RuleEngineController hooks
            initialiser.HooksAndAnchors.RegisterAnchor(new CheckDependenciesAnchor(initialiser));
            initialiser.HooksAndAnchors.RegisterAnchor(new ObjectRegistryAnchor(initialiser));
            initialiser.HooksAndAnchors.RegisterAnchor(new BeforeStartAnchor(initialiser));
            initialiser.HooksAndAnchors.RegisterAnchor(new AfterStartAnchor(initialiser));
            
            // Start the preinit stage for each controller, giving the engine instance beforehand
            // The preinit stage is reserved for setting up objects and taking references from the engine
            SelectiveDebug.StartTimer("Preinit");

            // Get all available addons
            addons = GetComponentsInChildren<RuleEngineAddon>();
            foreach (RuleEngineAddon addon in addons) { 
                addon.SetInitialiser(initialiser);

                // Allow addons to register their new anchors in the initialiser
                addon.RegisterAnchors();

            }

            // Allow addons to registers their hooks with any anchor
            foreach (RuleEngineAddon addon in addons)
                addon.RegisterHooks();

            // Preinit the initialiser to create the instance of the rule engine
            SelectiveDebug.LogInit("Preinit the initialiser");
            initialiser.Preinit(controllers, addons);

            // Extract a reference to the engine instance
            SelectiveDebug.LogInit("Extract the engine instance from the initialiser");
            E = initialiser.GetEngine();

            // Check addon dependencies
            initialiser.HooksAndAnchors.Process<CheckDependenciesAnchor>();

            // Set the engine reference and preinit all controllers, managers and addons
            SelectiveDebug.LogInit("Set the new engine and preinit each manager");
            foreach (RuleEngineAddon a in addons) {
                a.SetEngine(E);
                a.Preinit();
            }

            foreach (IController c in controllers) {
                c.SetEngine(E);
                c.Preinit();
            }

            HashSet<IManager> preinitialisedManagers = new HashSet<IManager>();
            foreach (IManager m in E.Managers) {
                if (preinitialisedManagers.Add(m)) {
                    m.SetEngine(E);
                    m.Preinit();
                }
            }
            
            SelectiveDebug.StopTimer("Preinit");
			
		}
		
		void Start() {

            SelectiveDebug.LogInit("Start");

            // Start the init stage
            // This is for everything else including interaction between controllers
            SelectiveDebug.StartTimer("Init");

            // Register object registries
            E.RegisterNewObjectRegistry(new IntegerObjectRegistry(E));
            E.RegisterNewObjectRegistry(new FloatObjectRegistry(E));
            initialiser.HooksAndAnchors.Process<ObjectRegistryAnchor>();

            // Initialise the managers
            SelectiveDebug.LogInit("Init the controllers, managers and addons");
            foreach (RuleEngineAddon a in addons)
                a.Init();

            foreach (IController c in controllers)
                c.Init();

            HashSet<IManager> initialisedManagers = new HashSet<IManager>();
            foreach (IManager m in E.Managers) {
                if (initialisedManagers.Add(m))
                    m.Init();
            }

            SelectiveDebug.LogInit("Init the initialiser");
            initialiser.Init(delegate {
                // Called once Init on the initialiser has been processed
                
                SelectiveDebug.StopTimer("Init");

                // When all effects have cleared
                new CallbackEffect().Init(delegate {

                    // Clear all modification stacks so that the board as is is locked in place for the first turn and can't be undone
                    E.RuleManager.ModStack.ClearModifications();

                    // Clear the judgement of the rule judge, so that all is reset to start
                    if (E.RuleManager.RuleJudge != null)
                        E.RuleManager.RuleJudge.ClearJudgement();

                    initialiser.HooksAndAnchors.Process<BeforeStartAnchor>();

                    // Re-enable all of the controllers so that their update functions get called
                    foreach (IController controller in controllers)
                        controller.SetEnabled(true);

                    // Finish the initialiser
                    initialiser.HooksAndAnchors.Process<AfterStartAnchor>();

                    // Check that all registered anchors have been processed
                    Assert.False("All registered anchors have been processed", initialiser.HooksAndAnchors.HasUnprocessedAnchors());

                    // Signal the game that it can now start accepting user input
                    SelectiveDebug.StopTimer("Initialiser");
                    SelectiveDebug.StopTimer("TotalSceneLoad");
                    
                    E.EffectFactory.NewEffect<IFinishedLoadingEffect>().Enqueue();

                }).Enqueue();

            });

            // Enable the thread and effect controller so that any threads or effects that have been queued up from Init may go ahead
            E.ThreadController.SetEnabled(true);
            E.EffectController.SetEnabled(true);

        }

    }
}