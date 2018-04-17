//#define TURNBASED_ADDON_DEBUG_MODE

using RuleEngine;
using System.Collections.Generic;
using UnityEngine;
using System;
using RuleEngineAddons.RulePanel;

namespace RuleEngineAddons.TurnBased {

	public class TurnBasedAddon : RuleEngineAddon {

        BoardManager BoardManager;
        TurnManager TurnManager;
        PlayerManager PlayerManager;

        public override void Preinit() {
        }

        public override void Init() {
        }

        public override void RegisterAnchors() {
            RegisterAnchor(new RegisterTurnBasedPlayersAnchor(initialiser));
            RegisterAnchor(new RegisterTurnBasedUnitTypesAnchor(initialiser));
            RegisterAnchor(new RegisterTurnBasedUnitTemplatesAnchor(initialiser));
            RegisterAnchor(new RegisterTurnBasedGroupingUnitTypesAnchor(initialiser));
            RegisterAnchor(new RegisterTurnBasedPositionTypesAnchor(initialiser));
            RegisterAnchor(new RegisterTurnBasedTileTypesAnchor(initialiser));
            RegisterAnchor(new BuildTurnBasedBoardAnchor(initialiser));
            RegisterAnchor(new RegisterTurnBasedUnitsAnchor(initialiser));
        }

        public override void RegisterHooks() {
            RegisterHookWithAnchor<CheckDependenciesAnchor>(CheckDependencies);
            RegisterHookWithAnchor<ManagerRegistrationAnchor>(ManagerRegistration);
            RegisterHookWithAnchor<RuleSorterRegistrationAnchor>(RuleSorterRegistration);
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
            RegisterHookWithAnchor<RulePanelSelectableObjectRegistrationAnchor>(RulePanelSelectableObjectRegistration);
            RegisterHookWithAnchor<RuleExecutionAnchor>(RuleExecution);
            RegisterHookWithAnchor<BeforeStartAnchor>(BeforeStart);
        }

        public void CheckDependencies(CheckDependenciesAnchor a) {
            a.RequireController<TurnController>();
        }

        public void ManagerRegistration(ManagerRegistrationAnchor a) {
            a.RegisterManager(new TurnManager(), delegate (IManager m) { TurnManager = (TurnManager) m; });
            a.RegisterManager(new BoardManager(), delegate (IManager m) { BoardManager = (BoardManager) m; });
            a.RegisterManager(new PlayerManager(), delegate (IManager m) { PlayerManager = (PlayerManager) m; });
            a.OverrideManager<ExecutionManager>(new TurnBasedExecutionManager());
        }
        
        public void RuleSorterRegistration(RuleSorterRegistrationAnchor a) {

            // Register the rule sorter to sort by units, if the turn based addon is present
            a.RegisterRuleSorter("ByUnitType", ByUnitTypeSorter);

        }

        public void RuleTypeRegistration(RuleTypeRegistrationAnchor a) {
            a.RegisterRuleType(TurnBasedExecutionManager.MOVEMENT);
            a.RegisterRuleType(TurnBasedExecutionManager.COLLISION);
            a.RegisterRuleType(TurnBasedExecutionManager.TURN);
        }

        public void ManagerConfiguration(ManagerConfigurationAnchor a) {

            TurnManager.TurnStateMachine = new StateMachine<TurnEvent, TurnState>(TurnState.Waiting, new TurnTransitionHandler(E));
            TurnManager.TurnStateMachine.Init();
            TurnManager.TurnStateMachine
                // Making a move, resolving collisions and switching players
                .AddTransition(TurnState.Waiting, TurnEvent.Make_Move, TurnState.Making_Move)
                .AddTransition(TurnState.Making_Move, TurnEvent.Wait_For_Input, TurnState.Waiting)
                .AddTransition(TurnState.Making_Move, TurnEvent.Resolve_Collisions, TurnState.Resolving_Collisions)
                .AddTransition(TurnState.Resolving_Collisions, TurnEvent.Wait_For_Input, TurnState.Waiting)
                .AddTransition(TurnState.Resolving_Collisions, TurnEvent.Switch_Players, TurnState.Switching_Players)
                .AddTransition(TurnState.Switching_Players, TurnEvent.Wait_For_Input, TurnState.Waiting)

                // Object placement
                .AddTransition(TurnState.Waiting, TurnEvent.Place_Object, TurnState.Placing_Object)
                .AddTransition(TurnState.Placing_Object, TurnEvent.Wait_For_Input, TurnState.Waiting);

            // Set easier direct access to rule types
            TurnBasedExecutionManager turnBasedExecutionManager = ((TurnBasedExecutionManager) E.ExecutionManager);
            turnBasedExecutionManager.Movement = E.RuleManager.GetRuleType(TurnBasedExecutionManager.MOVEMENT);
            turnBasedExecutionManager.Collision = E.RuleManager.GetRuleType(TurnBasedExecutionManager.COLLISION);
            turnBasedExecutionManager.Turn = E.RuleManager.GetRuleType(TurnBasedExecutionManager.TURN);

        }

        public void RuleExecutorRegistration(RuleExecutorRegistrationAnchor a) {
            a.RegisterRuleExecutorType<MovementRuleExecutor>();
            a.RegisterRuleExecutorType<CollisionRuleExecutor>();
            a.RegisterRuleExecutorType<TurnRuleExecutor>();
        }

        public void RuleExecutorFunctionRegistration(RuleExecutorFunctionRegistrationAnchor a) {

            // Register default executing methods. These should never be run since executing should be done using CRE classes
            a.RegisterDefaultVisitFunction<MovementRuleExecutor>(delegate (RuleExecutor RuleExecutor, RuleComponent obj) { Assert.Never("Unimplemented method for MovementRuleExecutor and " + obj.GetType().Name); } );
            a.RegisterDefaultVisitFunction<CollisionRuleExecutor>(delegate (RuleExecutor RuleExecutor, RuleComponent obj) { Assert.Never("Unimplemented method for CollisionRuleExecutor and " + obj.GetType().Name); } );
            a.RegisterDefaultVisitFunction<TurnRuleExecutor>(delegate (RuleExecutor RuleExecutor, RuleComponent obj) { Assert.Never("Unimplemented method for TurnRuleExecutor and " + obj.GetType().Name); } );

            a.RegisterValueVisitFunction<MovementRuleExecutor>(CommonTurnBasedRuleExecutor.VisitNonNullValue);
            a.RegisterValueVisitFunction<CollisionRuleExecutor>(CollisionRuleExecutor.VisitNonNullValueOverride);
            a.RegisterValueVisitFunction<TurnRuleExecutor>(CommonTurnBasedRuleExecutor.VisitNonNullValue);

            a.RegisterSpecificVisitFunction<NullValue, MovementRuleExecutor>(CommonTurnBasedRuleExecutor.VisitNullValue);
            a.RegisterSpecificVisitFunction<NullValue, CollisionRuleExecutor>(CommonTurnBasedRuleExecutor.VisitNullValue);
            a.RegisterSpecificVisitFunction<NullValue, TurnRuleExecutor>(CommonTurnBasedRuleExecutor.VisitNullValue);

            a.RegisterSpecificVisitFunction<VoidStatement, MovementRuleExecutor>(CommonTurnBasedRuleExecutor.VisitVoid);
            a.RegisterSpecificVisitFunction<VoidStatement, CollisionRuleExecutor>(CommonTurnBasedRuleExecutor.VisitVoid);
            a.RegisterSpecificVisitFunction<VoidStatement, TurnRuleExecutor>(CommonTurnBasedRuleExecutor.VisitVoid);

            // Register RulePanelAddon RuleExecutor functions if the rule panel addon is being used
            if (E.HasAddon("RulePanelAddon")) {
                
                a.RegisterSpecificVisitFunction<DeclareWinnerStatement, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitDeclareWinnerStatement);
                a.RegisterSpecificVisitFunction<DeclareLoserStatement, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitDeclareLoserStatement);
                a.RegisterSpecificVisitFunction<MoveStatement, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitMoveStatement);
                a.RegisterSpecificVisitFunction<RemoveUnitStatement, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitRemoveUnitStatement);
                a.RegisterSpecificVisitFunction<CancelMoveStatement, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitCancelMoveStatement);
                a.RegisterSpecificVisitFunction<ReplaceUnitStatement, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitReplaceUnitStatement);
                a.RegisterSpecificVisitFunction<AddTileStatement, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitAddTileStatement);
                a.RegisterSpecificVisitFunction<GetXExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitGetXExpression);
                a.RegisterSpecificVisitFunction<GetYExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitGetYExpression);
                a.RegisterSpecificVisitFunction<IsThreatenedExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitIsThreatenedExpression);
                a.RegisterSpecificVisitFunction<IsTileOccupiedRExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitIsTileOccupiedRExpression);
                a.RegisterSpecificVisitFunction<PlayerOfUnitExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitPlayerOfUnitExpression);
                a.RegisterSpecificVisitFunction<IsCurrentPlayerExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitIsCurrentPlayerExpression);
                a.RegisterSpecificVisitFunction<IsPreviousPlayerExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitIsPreviousPlayerExpression);
                a.RegisterSpecificVisitFunction<TypeOfUnitExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitTypeOfUnitExpression);
                a.RegisterSpecificVisitFunction<TypeOfTileExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitTypeOfTileExpression);
                a.RegisterSpecificVisitFunction<TileOfUnitExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitTileOfUnitExpression);
                a.RegisterSpecificVisitFunction<GetCurrentPlayerExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitGetCurrentPlayerExpression);
                a.RegisterSpecificVisitFunction<DirectionAdditionExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitDirectionAdditionExpression);
                a.RegisterSpecificVisitFunction<NoRuleUsableExpression, RuleRenderingExecutor>(TurnBasedRuleRenderingRuleExecutor.VisitNoRuleUsableExpression);

            }

        }

        public void EffectInterfaceRegistration(EffectInterfaceRegistrationAnchor a) {
            a.RegisterEffectInterface<IAddPositionEffect>();
            a.RegisterEffectInterface<IAddTileEffect>();
            a.RegisterEffectInterface<IAddUnitEffect>();
            a.RegisterEffectInterface<IAnnouceLosersEffect>();
            a.RegisterEffectInterface<IAnnouceWinnersEffect>();
            a.RegisterEffectInterface<IConfigureNewBoardObjectEffect>();
            a.RegisterEffectInterface<IInstantMoveBoardObjectEffect>();
            a.RegisterEffectInterface<IMoveAndReturnUnitEffect>();
            a.RegisterEffectInterface<IMoveUnitEffect>();
            a.RegisterEffectInterface<IRemovePositionEffect>();
            a.RegisterEffectInterface<IRemoveTileEffect>();
            a.RegisterEffectInterface<IRemoveUnitEffect>();
            a.RegisterEffectInterface<IStartGameObjectPlacementEffect>();
            a.RegisterEffectInterface<IStopGameObjectPlacementEffect>();
            a.RegisterEffectInterface<IUpdateTileOfUnitEffect>();
            a.RegisterEffectInterface<ITurnChangeEffect>();
        }

        public void EffectImplementationRegistration(EffectImplementationRegistrationAnchor a) {
            a.RegisterEffectImplementation<IStartedLoadingEffect, StartedLoadingEffect>();
            a.RegisterEffectImplementation<IFinishedLoadingEffect, FinishedLoadingEffect>();
        }

        public void PlayerRegistration(PlayerRegistrationAnchor a) {

            ProcessAnchor<RegisterTurnBasedPlayersAnchor>();

            // Verify the players
            Assert.True("There are players", PlayerManager.PlayersByPosition.Count > 0);
            
        }

        public void ObjectTypeRegistration(ObjectTypeRegistrationAnchor a) {

            new UnitTypeCreator_TS(E, "Any")
                .SetToGroupingType()
                .Finalise();

            ProcessAnchor<RegisterTurnBasedUnitTypesAnchor>();
            ProcessAnchor<RegisterTurnBasedUnitTemplatesAnchor>();
            
            // Get and register a player specific template for each of the unit object type value
            //foreach (UnitObjectTypeValue uotv in BoardManager.UnitRegistry.AllObjectTypeValuesEnumerable_TS()) {
            //    foreach (Player player in PlayerManager.Players) {
            //        GameObject templateForUnitAndPlayer = BoardManager.UnitRegistry.GetTemplateByPlayerAndUnitID_TS
            //        uotv.AddTemplate(uotv.UnitType.GetTemplate(player));
            //    }
            //}

            ProcessAnchor<RegisterTurnBasedGroupingUnitTypesAnchor>();
            ProcessAnchor<RegisterTurnBasedPositionTypesAnchor>();
            ProcessAnchor<RegisterTurnBasedTileTypesAnchor>();

            // Build profile of tile types
            float RarityTotal = 0f;
            foreach (TileType type in BoardManager.TileRegistry.AllObjectTypesEnumerable_TS()) {
                RarityTotal += type.Rarity;
            }
            E.Set_TS("RarityTotal", RarityTotal);

        }

        public void ObjectRegistration(ObjectRegistrationAnchor a) {

            ProcessAnchor<BuildTurnBasedBoardAnchor>();
            ProcessAnchor<RegisterTurnBasedUnitsAnchor>();

            // Clear unit moves
            foreach (Unit unit in BoardManager.UnitRegistry.AllObjectsEnumerable_TS()) {
                unit.ClearMoves_TS();
            }

        }

        public void VariableRegistration(VariableRegistrationAnchor a) {

            UnitObjectTypeValue AnyUnitValue = BoardManager.UnitRegistry.GetObjectTypeValueByID_TS("Any");

            // Define the type of the variables for the moving and target units in movement and collision cases
            a.NewVariable("Moving unit", AnyUnitValue);
            a.NewVariable("Target unit", AnyUnitValue);

        }
        
        public void RulePanelSelectableObjectRegistration(RulePanelSelectableObjectRegistrationAnchor a) {

            // Statements
            a.RegisterSelectableObject(new MoveStatement(E));
            a.RegisterSelectableObject(new RemoveUnitStatement(E));
            a.RegisterSelectableObject(new CancelMoveStatement(E));
            a.RegisterSelectableObject(new DeclareWinnerStatement(E));

            // Expressions
            a.RegisterSelectableObject(new GetXExpression(E));
            a.RegisterSelectableObject(new GetYExpression(E));
            a.RegisterSelectableObject(new IsThreatenedExpression(E));
            a.RegisterSelectableObject(new IsTileOccupiedRExpression(E));
            a.RegisterSelectableObject(new IsCurrentPlayerExpression(E));
            a.RegisterSelectableObject(new IsPreviousPlayerExpression(E));
            a.RegisterSelectableObject(new PlayerOfUnitExpression(E));
            a.RegisterSelectableObject(new TypeOfUnitExpression(E));
            a.RegisterSelectableObject(new TileOfUnitExpression(E));
            a.RegisterSelectableObject(new TypeOfTileExpression(E));
            a.RegisterSelectableObject(new GetCurrentPlayerExpression(E));
            a.RegisterSelectableObject(new DirectionAdditionExpression(E));
            a.RegisterSelectableObject(new NoRuleUsableExpression(E));

            // Direction values
            a.RegisterSelectableObject(new DirectionValue(E, Vector3.zero));
            a.RegisterSelectableObject(new DirectionValue(E, Direction.Forward));
            a.RegisterSelectableObject(new DirectionValue(E, Direction.Backward));
            a.RegisterSelectableObject(new DirectionValue(E, Direction.Right));
            a.RegisterSelectableObject(new DirectionValue(E, Direction.Left));
            a.RegisterSelectableObject(new DirectionValue(E, Direction.Forward_right));
            a.RegisterSelectableObject(new DirectionValue(E, Direction.Forward_left));
            a.RegisterSelectableObject(new DirectionValue(E, Direction.Backward_right));
            a.RegisterSelectableObject(new DirectionValue(E, Direction.Backward_left));

        }
        
        public void RuleExecution(RuleExecutionAnchor a) {

            bool MoveCancelled;

            TurnBasedExecutionManager TurnBasedExecutionManager = E.GetManager<TurnBasedExecutionManager>();
            TurnBasedExecutionManager.ExecuteMovementRulesSynchronously();
            TurnBasedExecutionManager.ExecuteTurnRulesSynchronously(out MoveCancelled);

            if (MoveCancelled)
                throw new Exception("Move was cancelled by the turn rules on first execution");

        }

        public void BeforeStart(BeforeStartAnchor a) {
            
            // Clear modification stack so that the board is locked in place for the first turn and can't be undone
            E.ModificationManager.ClearModifications_TS();

            // Setup the turn manager
            TurnManager.SetupFirstTurn();

#if TURNBASED_ADDON_DEBUG_MODE

            if (E.HasAddon<RulePanelAddon>()) {
                Action ExecuteMovementRulesAction = delegate {
                    ((TurnBasedExecutionManager) E.ExecutionManager).ExecuteMovementRules();
                };
                Action ExecuteCollisionRulesAction = delegate {
                    ((TurnBasedExecutionManager) E.ExecutionManager).ExecuteMovementRules();
                };
                Action ExecuteTurnRulesAction = delegate {
                    ((TurnBasedExecutionManager) E.ExecutionManager).ExecuteMovementRules();
                };

                E.GetManager<PanelManager>().ActionStackSubsystem.Stacks["GameStack"].AddButtonItem("MovementRuleExecute", "Run Movement Rules", ExecuteMovementRulesAction, false, true, true);
                E.GetManager<PanelManager>().ActionStackSubsystem.Stacks["GameStack"].AddButtonItem("CollisionRuleExecute", "Run Collision Rules", ExecuteCollisionRulesAction, false, true, true);
                E.GetManager<PanelManager>().ActionStackSubsystem.Stacks["GameStack"].AddButtonItem("TurnRuleExecute", "Run Turn Rules", ExecuteTurnRulesAction, false, true, true);
            }

#endif

        }

        private IEnumerable<int> ByUnitTypeSorter(RuleExecutor RuleExecutor, RuleType RuleType) {
            Dictionary<UnitType, List<int>> SortedByUnitType = null;

            UnitType AnyUnitType = BoardManager.UnitRegistry.GetObjectTypeByID_TS("Any");

            SortedByUnitType = new Dictionary<UnitType, List<int>>();
            foreach (UnitType t in BoardManager.UnitRegistry.AllObjectTypesEnumerable_TS()) {
                SortedByUnitType[t] = new List<int>();
            }

            // Sort the rules into their types
            Type TypeOfUnitTypeValue = typeof(UnitObjectTypeValue);

            RuleList list = E.RuleManager.RuleTypeToList[RuleType];
            for (int ruleIndex = 0; ruleIndex < list.Count; ruleIndex++) {
                Rule rule = list[ruleIndex];

                if (rule.VariablesByType.ContainsKey(TypeOfUnitTypeValue)) {

                    foreach (Variable v in rule.VariablesByType[TypeOfUnitTypeValue]) {
                        Assert.Same("Variable type is UnitObjectTypeValue", typeof(UnitObjectTypeValue), v.VariableType.GetType());

                        UnitType type = (UnitType) ((UnitObjectTypeValue) v.VariableType).TypeInstance;
                        SortedByUnitType[type].Add(ruleIndex);

                    }

                } else {

                    // For rules without the presence of unit types, we use the unit type Any
                    SortedByUnitType[AnyUnitType].Add(ruleIndex);

                }

            }

            foreach (UnitType type in BoardManager.UnitRegistry.AllObjectTypesEnumerable_TS()) {
                ((TurnBasedRuleExecutor) RuleExecutor).CurrentUnitType = type;
                List<int> indices = SortedByUnitType[type];
                for (int i = 0; i < indices.Count; i++)
                    yield return indices[i];
            }

        }

    }

}
