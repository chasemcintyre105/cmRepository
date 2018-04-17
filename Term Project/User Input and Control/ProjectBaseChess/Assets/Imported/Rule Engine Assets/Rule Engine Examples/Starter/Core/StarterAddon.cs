using RuleEngine;
using RuleEngineAddons.RulePanel;
using RuleEngineAddons.TurnBased;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineExamples.StarterProject {

    /**
     * This is a sample project that contains a very simple implementation of as many features of the Rule Engine as possible
     * See the NewManager for some extra code samples
     */
    public class StarterAddon : RuleEngineAddon {
        
        public static string NewTurnBasedUnitTypeID = "NewTurnBasedUnitType";
        public static string NewTileTypeID = "NewTileType";
        public static string NewPositionTypeID = "NewPositionType";
        public static string NewRuleTypeID = "NewRuleType";

        NewManager newManager;

        // Core
        public NewObjectRegistry newObjectRegistry;
        public NewObject newObject;
        public NewObjectType newObjectType;
        public NewObjectValue newObjectValue;
        public NewObjectTypeValue newObjectTypeValue;
        public Rule newRule;

        // Turn based
        public Player newTurnBasedPlayer;
        public PlayerPosition newTurnBasedPlayerPosition;
        public UnitType newTurnBasedUnitType;
        public TileType newTurnBasedTileType;
        public PositionType newTurnBasedPositionType;
        public Dictionary<Vector2, Tile> tiles;

        public override void Preinit() {
            tiles = new Dictionary<Vector2, Tile>();
        }

        public override void Init() {
        }

        public override void RegisterAnchors() {
            RegisterAnchor<NewAnchor>(new NewAnchor(initialiser)); // Processed as an example in the AfterStart anchor.
        }

        public override void RegisterHooks() {

            // Core hooks
            RegisterHookWithAnchor<CheckDependenciesAnchor>(CheckDependencies);
            RegisterHookWithAnchor<ObjectRegistryAnchor>(ObjectRegistry);
            RegisterHookWithAnchor<ManagerRegistrationAnchor>(ManagerRegistration);
            RegisterHookWithAnchor<RuleSorterRegistrationAnchor>(RuleSorterRegistration);
            RegisterHookWithAnchor<RuleVariableIteratorRegistrationAnchor>(RuleVariableIteratorRegistration);
            RegisterHookWithAnchor<RuleTypeRegistrationAnchor>(RuleTypeRegistration);
            RegisterHookWithAnchor<ManagerConfigurationAnchor>(ManagerConfiguration);
            RegisterHookWithAnchor<RuleExecutorRegistrationAnchor>(RuleExecutorRegistration);
            RegisterHookWithAnchor<RuleExecutorFunctionRegistrationAnchor>(RuleExecutorFunctionRegistration);
            RegisterHookWithAnchor<EffectInterfaceRegistrationAnchor>(EffectInterfaceRegistration);
            RegisterHookWithAnchor<EffectImplementationRegistrationAnchor>(EffectImplementationRegistration);
            RegisterHookWithAnchor<PlayerRegistrationAnchor>(PlayerRegistration);
            RegisterHookWithAnchor<ObjectTypeRegistrationAnchor>(ObjectTypeRegistration);
            RegisterHookWithAnchor<ObjectRegistrationAnchor>(ObjectRegistration);
            RegisterHookWithAnchor<VariableRegistrationAnchor>(VariableRegistration);
            RegisterHookWithAnchor<RuleRegistrationAnchor>(RuleRegistration);
            RegisterHookWithAnchor<RuleExecutionAnchor>(RuleExecution);
            RegisterHookWithAnchor<BeforeStartAnchor>(BeforeStart);
            RegisterHookWithAnchor<AfterStartAnchor>(AfterStart);

            // Rule Panel Addon hooks
            RegisterHookWithAnchor<ConfigureRuleJudgeClassAnchor>(ConfigureRuleJudgeClass);
            RegisterHookWithAnchor<ActionStackConfigurationAnchor>(ActionStackConfiguration);
            RegisterHookWithAnchor<RulePanelSelectableObjectRegistrationAnchor>(RulePanelSelectableObjectRegistration);

            // Turn Based Addon hooks
            RegisterHookWithAnchor<RegisterTurnBasedPlayersAnchor>(RegisterTurnBasedPlayers);
            RegisterHookWithAnchor<RegisterTurnBasedUnitTypesAnchor>(RegisterTurnBasedUnitTypes);
            RegisterHookWithAnchor<RegisterTurnBasedUnitTemplatesAnchor>(RegisterTurnBasedUnitTemplates);
            RegisterHookWithAnchor<RegisterTurnBasedGroupingUnitTypesAnchor>(RegisterTurnBasedGroupingUnitTypes);
            RegisterHookWithAnchor<RegisterTurnBasedPositionTypesAnchor>(RegisterTurnBasedPositionTypes);
            RegisterHookWithAnchor<RegisterTurnBasedTileTypesAnchor>(RegisterTurnBasedTileTypes);
            RegisterHookWithAnchor<BuildTurnBasedBoardAnchor>(BuildTurnBasedBoard);
            RegisterHookWithAnchor<RegisterTurnBasedUnitsAnchor>(RegisterTurnBasedUnits);

            // Custom hook
            RegisterHookWithAnchor<NewAnchor>(NewAnchorHandler);

        }

        // Core hooks
        public void CheckDependencies(CheckDependenciesAnchor a) {
            a.RequireAddon<RulePanelAddon>();
            a.RequireAddon<TurnBasedAddon>();

            a.RequireController<NewController>();
        }

        public void ObjectRegistry(ObjectRegistryAnchor a) {
            a.RegisterNewObjectRegistry(new NewObjectRegistry(E));

            // This may be retrieved elsewhere using the type of the base object class
            newObjectRegistry = (NewObjectRegistry) E.ObjectRegistries[typeof(NewObject)];

            // Alternatively the ObjectRegistry may be used in its most general form and registered using:
            
            //SpecificObjectRegistry<NewObject, NewObjectType, NewObjectValue, NewObjectTypeValue> newRegistry = a.RegisterNewObjectRegistry<NewObject, NewObjectType, NewObjectValue, NewObjectTypeValue>(
            //delegate (Engine E, NewObject o, NewObjectTypeValue otv) {
            //    return new NewObjectValue(E, otv, o);
            //}, delegate (Engine E, NewObjectType ot) {
            //    return new NewObjectTypeValue(E, ot);
            //});

        }

        public void ManagerRegistration(ManagerRegistrationAnchor a) {
            a.RegisterManager(new NewManager(), delegate (IManager m) { newManager = (NewManager) m; } );
        }

        public void RuleSorterRegistration(RuleSorterRegistrationAnchor a) {
            a.RegisterRuleSorter("NewRuleSorter", NewRuleSorter);
        }

        public IEnumerable<int> NewRuleSorter(RuleExecutor RuleExecutor, RuleType RuleType) {

            // Return indices in sequential order (Same as "Sequential")
            for (int index = 0; index < E.RuleManager.RuleTypeToList[RuleType].Count; index++)
                yield return index; 

        }

        public void RuleVariableIteratorRegistration(RuleVariableIteratorRegistrationAnchor a) {
            a.RegisterRuleVariableIterator("NewRuleVariableIterator", NewRuleVariableIterator);
        }

        public IEnumerable<bool> NewRuleVariableIterator(RuleExecutor RuleExecutor, Rule Rule) {

            // Return true just once before finishing (Same as "NoChange")
            yield return true;

        }

        public void RuleTypeRegistration(RuleTypeRegistrationAnchor a) {
            a.RegisterRuleType(NewRuleTypeID);
        }

        public void ManagerConfiguration(ManagerConfigurationAnchor a) {
            // Configure newManager
        }

        public void RuleExecutorRegistration(RuleExecutorRegistrationAnchor a) {
            a.RegisterRuleExecutorType<NewRuleExecutor>();
        }

        public void RuleExecutorFunctionRegistration(RuleExecutorFunctionRegistrationAnchor a) {
            a.RegisterDefaultVisitFunction<NewRuleExecutor>(NewRuleExecutor.DefaultVisitFunction);
            a.RegisterStatementVisitFunction<NewRuleExecutor>(NewRuleExecutor.StatementVisitFunction);
            a.RegisterExpressionVisitFunction<NewRuleExecutor>(NewRuleExecutor.ExpressionVisitFunction);
            a.RegisterValueVisitFunction<NewRuleExecutor>(NewRuleExecutor.ValueVisitFunction);
            a.RegisterNullVisitFunction<NewRuleExecutor>(NewRuleExecutor.NullVisitFunction);
            a.RegisterVoidVisitFunction<NewRuleExecutor>(NewRuleExecutor.VoidVisitFunction);
            a.RegisterSpecificVisitFunction<NewStatement, NewRuleExecutor>(NewRuleExecutor.NewStatementVisitFunction);
        }

        public void EffectInterfaceRegistration(EffectInterfaceRegistrationAnchor a) {
            a.RegisterEffectInterface<INewEffect>();
        }

        public void EffectImplementationRegistration(EffectImplementationRegistrationAnchor a) {
            a.RegisterEffectImplementation<INewEffect, NewEffect>();
            a.RegisterEffectImplementation<IDisplayMessageEffect, NewDisplayMessageEffect>();
        }

        public void PlayerRegistration(PlayerRegistrationAnchor a) {
            // See turn based player registration for this example
        }

        public void ObjectTypeRegistration(ObjectTypeRegistrationAnchor a) {
            newObjectType = new NewObjectType("New object type");
            newObjectTypeValue = new NewObjectTypeValue(E, newObjectType);

            // Register type
            newObjectRegistry.RegisterObjectType_TS(newObjectType);

        }

        public void ObjectRegistration(ObjectRegistrationAnchor a) {
            newObject = new NewObject(newObjectRegistry.GenerateObjectUID_TS(), newObjectType);
            newObjectValue = new NewObjectValue(E, newObjectTypeValue, newObject);

            // Register object
            newObjectRegistry.RegisterObject_TS(newObject);

        }

        public void VariableRegistration(VariableRegistrationAnchor a) {
            a.NewVariable("NewGlobalVariable", newObjectTypeValue);
        }

        public void RuleRegistration(RuleRegistrationAnchor a) {
            new RuleCreator(E, E.RuleManager.RuleIDToType[NewRuleTypeID])
                .SetName("NewRule")
                .SetUID("NewRule1")
                .StartRule()
                    .AddAndConfigure<NewStatement>().Set<NewExpression>()
                .ReturnTo<BaseBlockCreator>()
                .FinishRule()
                .Finalise(out newRule);
        }

        public void RuleExecution(RuleExecutionAnchor a) {

            // Synchronously execute just the single testing rule
            newManager.ExecuteNewRuleSynchronously("NewRule1");

            // Otherwise to execute all the rules of a type
            //newManager.ExecuteNewRulesSynchronously();

            // And to do the same, but asynchronously
            //newManager.ExecuteNewRuleAsynchronously("NewRule1");
            //newManager.ExecuteNewRulesAsynchronously();

        }

        public void BeforeStart(BeforeStartAnchor a) {

        }

        public void AfterStart(AfterStartAnchor a) {

            // As an example, the new anchor is processed here
            ProcessAnchor<NewAnchor>();

        }


        // Rule Panel Addon hooks
        public void ConfigureRuleJudgeClass(ConfigureRuleJudgeClassAnchor a) {
            a.SetRuleJudge<DefaultRuleJudge>();
        }

        public void ActionStackConfiguration(ActionStackConfigurationAnchor a) {

            a.ActionStackManager.Stacks["GameStack"].AddItem_TS(new ActionStackItemProfile() {
                ID = "Execute",
                Type = ActionStackItemProfile.StackOptionType.Button,
                Title = "Execute new rules",
                DestroyOnClick = false,
                Permanent = true,
                OnClick = delegate {
                    newManager.ExecuteNewRulesAsynchronously();
                }
            });

        }

        public void RulePanelSelectableObjectRegistration(RulePanelSelectableObjectRegistrationAnchor a) {

            // All rule components instances for the selectable object register should not be used elsewhere
            a.RegisterSelectableObject(new NewStatement(E));
            a.RegisterSelectableObject(new NewExpression(E));
            a.RegisterSelectableObject(new NewObjectValue(E, newObjectTypeValue, newObject));

            // The only excepation are types that are shared and not changed so a new instance is not necessary
            a.RegisterSelectableObject(newObjectTypeValue);

        }


        // Turn Based Addon hooks
        public void RegisterTurnBasedPlayers(RegisterTurnBasedPlayersAnchor a) {

            // Turn Based Addon player implementation:
            newTurnBasedPlayerPosition = new PlayerPosition(Vector3.zero, 0, 0, false);
            newTurnBasedPlayer = a.AddPlayer("NewPlayer", "Scarlet", new Color(1, 0, 0), newTurnBasedPlayerPosition, new GameObject("Player Turn Display"), new GameObject("Player Coordinate Origin"));

            a.SetupCoordinateOriginMarker(newTurnBasedPlayerPosition);
        }

        public void RegisterTurnBasedUnitTypes(RegisterTurnBasedUnitTypesAnchor a) {
            new UnitTypeCreator_TS(E, NewTurnBasedUnitTypeID)
                .Finalise(out newTurnBasedUnitType);
        }

        public void RegisterTurnBasedUnitTemplates(RegisterTurnBasedUnitTemplatesAnchor a) {
            GameObject NewUnitTemplate = E.GetController<NewController>().Templates.NewObjectTemplate;
            a.AddUnitTypeTemplate(newTurnBasedPlayer, NewTurnBasedUnitTypeID, NewUnitTemplate);
        }

        public void RegisterTurnBasedGroupingUnitTypes(RegisterTurnBasedGroupingUnitTypesAnchor a) {
            // Used for non-individual unit types such as "Any unit". See Chess example
        }

        public void RegisterTurnBasedPositionTypes(RegisterTurnBasedPositionTypesAnchor a) {
            new PositionTypeCreator_TS(E, NewPositionTypeID)
                .Finalise(out newTurnBasedPositionType);
        }

        public void RegisterTurnBasedTileTypes(RegisterTurnBasedTileTypesAnchor a) {
            new TileTypeCreator_TS(E, NewTileTypeID)
                .AddPositionType(Vector3.zero, newTurnBasedPositionType)
                .Finalise(out newTurnBasedTileType);
        }

        public void BuildTurnBasedBoard(BuildTurnBasedBoardAnchor a) {
            a.PlaceSingleTile(Vector2.zero, newTurnBasedTileType, tiles);
            a.PlaceSquareGridOfTiles(new Vector2(1, 0), new Vector2(3, 3), newTurnBasedTileType, tiles);
            a.PlaceSquareGridOfTiles(new Vector2(4, 0), new Vector2(3, 3), delegate (Vector2 position) {
                return newTurnBasedTileType; // Return tile type based on position
            }, tiles);
        }

        public void RegisterTurnBasedUnits(RegisterTurnBasedUnitsAnchor a) {
            new UnitCreator_TS(E, Vector3.zero, newTurnBasedPlayer, NewTurnBasedUnitTypeID)
                .Finalise();
        }


        // Custom hook
        public void NewAnchorHandler(NewAnchor a) {

        }

    }

}

