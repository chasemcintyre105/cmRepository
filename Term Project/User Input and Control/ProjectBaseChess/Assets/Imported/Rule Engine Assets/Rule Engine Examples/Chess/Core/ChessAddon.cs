using RuleEngine;
using RuleEngineAddons.RulePanel;
using RuleEngineAddons.TurnBased;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineExamples.Chess {

    public class ChessAddon : RuleEngineAddon {

        public PlayerPosition LeftSide;
        public PlayerPosition RightSide;

        public PositionType CenterPosition;
        public PositionType EdgePosition;
        public PositionType VertexPosition;

        public Dictionary<Vector2, Tile> tiles;

        public Player White;
        public Player Black;

        public UnitType PawnType;
        public UnitType RookType;
        public UnitType KnightType;
        public UnitType BishopType;
        public UnitType QueenType;
        public UnitType KingType;
        public UnitType AnyType;

        public UnitObjectTypeValue PawnTypeValue;
        public UnitObjectTypeValue RookTypeValue;
        public UnitObjectTypeValue KnightTypeValue;
        public UnitObjectTypeValue BishopTypeValue;
        public UnitObjectTypeValue QueenTypeValue;
        public UnitObjectTypeValue KingTypeValue;
        public UnitObjectTypeValue AnyTypeValue;

        private List<Direction> straightDirections;
        private List<Direction> diagonalDirections;

        private Vector3[] SquarePositionOffsets;

        private RuleType _MovementRuleType = null;
        public RuleType MovementRuleType {
            get {
                if (_MovementRuleType == null)
                    _MovementRuleType = ((TurnBasedExecutionManager) E.ExecutionManager).Movement;
                return _MovementRuleType;
            }
        }

        private RuleType _CollisionRuleType = null;
        public RuleType CollisionRuleType {
            get {
                if (_CollisionRuleType == null)
                    _CollisionRuleType = ((TurnBasedExecutionManager) E.ExecutionManager).Collision;
                return _CollisionRuleType;
            }
        }

        private RuleType _TurnRuleType = null;
        public RuleType TurnRuleType {
            get {
                if (_TurnRuleType == null)
                    _TurnRuleType = ((TurnBasedExecutionManager) E.ExecutionManager).Turn;
                return _TurnRuleType;
            }
        }

        private BoardManager BoardManager;
        private TurnController BoardController;
        private RulePanelManager RulePanelManager;
        private ChessSettingsController CommonSettingsController;
        private TurnBasedExecutionManager TurnBasedExecutionManager;

        public override void Preinit() {

            tiles = new Dictionary<Vector2, Tile>();

            LeftSide = new PlayerPosition(new Vector3(1, 1, 1), 0, 0, true);
            RightSide = new PlayerPosition(new Vector3(8, 8, 1), 180, 0, true);

            straightDirections = new List<Direction>();
            straightDirections.Add(Direction.Forward);
            straightDirections.Add(Direction.Backward);
            straightDirections.Add(Direction.Right);
            straightDirections.Add(Direction.Left);

            diagonalDirections = new List<Direction>();
            diagonalDirections.Add(Direction.Backward_left);
            diagonalDirections.Add(Direction.Backward_right);
            diagonalDirections.Add(Direction.Forward_left);
            diagonalDirections.Add(Direction.Forward_right);
            
        }

        public override void Init() {
            BoardManager = E.GetManager<BoardManager>();
            RulePanelManager = E.GetManager<RulePanelManager>();
            BoardController = E.GetController<TurnController>();
            CommonSettingsController = E.GetController<ChessSettingsController>();
            TurnBasedExecutionManager = ((TurnBasedExecutionManager) E.ExecutionManager);
        }

        public override void RegisterAnchors() {
        }

        public override void RegisterHooks() {
            RegisterHookWithAnchor<CheckDependenciesAnchor>(CheckDependencies);
            RegisterHookWithAnchor<ManagerRegistrationAnchor>(ManagerRegistration);
            RegisterHookWithAnchor<ManagerConfigurationAnchor>(ManagerConfiguration);
            RegisterHookWithAnchor<ConfigureRuleJudgeClassAnchor>(ConfigureRuleJudgeClass);
            RegisterHookWithAnchor<EffectInterfaceRegistrationAnchor>(EffectInterfaceRegistration);
            RegisterHookWithAnchor<EffectImplementationRegistrationAnchor>(EffectImplementationRegistration);
            RegisterHookWithAnchor<RegisterTurnBasedPlayersAnchor>(RegisterTurnBasedPlayers);
            RegisterHookWithAnchor<RegisterTurnBasedUnitTypesAnchor>(RegisterTurnBasedUnitTypes);
            RegisterHookWithAnchor<RegisterTurnBasedUnitTemplatesAnchor>(RegisterTurnBasedUnitTemplates);
            RegisterHookWithAnchor<RegisterTurnBasedGroupingUnitTypesAnchor>(RegisterTurnBasedGroupingUnitTypes);
            RegisterHookWithAnchor<RegisterTurnBasedPositionTypesAnchor>(RegisterTurnBasedPositionTypes);
            RegisterHookWithAnchor<RegisterTurnBasedTileTypesAnchor>(RegisterTurnBasedTileTypes);
            RegisterHookWithAnchor<BuildTurnBasedBoardAnchor>(BuildTurnBasedBoard);
            RegisterHookWithAnchor<RegisterTurnBasedUnitsAnchor>(RegisterTurnBasedUnits);
            RegisterHookWithAnchor<RuleRegistrationAnchor>(RuleRegistration);
            RegisterHookWithAnchor<BeforeStartAnchor>(BeforeStart);
        }

        public void CheckDependencies(CheckDependenciesAnchor a) {
            a.RequireAddon<RulePanelAddon>();
            a.RequireAddon<TurnBasedAddon>();

            a.RequireController<TurnController>();
            a.RequireController<KeyStrokeController>();
            a.RequireController<CameraController>();
            a.RequireController<ChessSettingsController>();
        }

        public void ManagerRegistration(ManagerRegistrationAnchor a) {
            a.RegisterManager(new GUIManager());
        }

        public void ManagerConfiguration(ManagerConfigurationAnchor a) {
            
            // Register selection option exclusions
            RulePanelManager.RegisterSelectionOptionExclusion(TurnBasedExecutionManager.Movement, typeof(CancelMoveStatement));
            RulePanelManager.RegisterSelectionOptionExclusion(TurnBasedExecutionManager.Movement, typeof(ReplaceUnitStatement));
            RulePanelManager.RegisterSelectionOptionExclusion(TurnBasedExecutionManager.Movement, typeof(NoRuleUsableExpression));
            RulePanelManager.RegisterSelectionOptionExclusion(TurnBasedExecutionManager.Movement, typeof(DeclareWinnerStatement));
            RulePanelManager.RegisterSelectionOptionExclusion(TurnBasedExecutionManager.Collision, typeof(NoRuleUsableExpression));
            RulePanelManager.RegisterSelectionOptionExclusion(TurnBasedExecutionManager.Turn, typeof(MoveStatement));
            
        }

        public void ConfigureRuleJudgeClass(ConfigureRuleJudgeClassAnchor a) {
            a.SetRuleJudge<LimitedRuleJudge>();
        }

        public void RegisterTurnBasedPlayers(RegisterTurnBasedPlayersAnchor a) {
            foreach (ChessSettingsController.PlayerColourProfile profile in CommonSettingsController.BoardTemplates.PlayerColourProfiles) {
                if (profile.Name == "White") {
                    if (profile.Material != null)
                        White = a.AddPlayer("Player 1", "White", profile.Material, LeftSide, CommonSettingsController.BoardObjects.Player1TurnDisplay, a.SetupCoordinateOriginMarker(LeftSide));
                    else
                        White = a.AddPlayer("Player 1", "White", profile.Colour, LeftSide, CommonSettingsController.BoardObjects.Player1TurnDisplay, a.SetupCoordinateOriginMarker(LeftSide));
                }
                if (profile.Name == "Black") {
                    if (profile.Material != null)
                        Black = a.AddPlayer("Player 2", "Black", profile.Material, RightSide, CommonSettingsController.BoardObjects.Player2TurnDisplay, a.SetupCoordinateOriginMarker(RightSide));
                    else
                        Black = a.AddPlayer("Player 2", "Black", profile.Colour, RightSide, CommonSettingsController.BoardObjects.Player2TurnDisplay, a.SetupCoordinateOriginMarker(RightSide));
                }
            }
        }

        public void RegisterTurnBasedUnitTypes(RegisterTurnBasedUnitTypesAnchor a) {

            new UnitTypeCreator_TS(E, "Pawn").Finalise(out PawnType);
            new UnitTypeCreator_TS(E, "Rook").Finalise(out RookType);
            new UnitTypeCreator_TS(E, "Knight").Finalise(out KnightType);
            new UnitTypeCreator_TS(E, "Bishop").Finalise(out BishopType);
            new UnitTypeCreator_TS(E, "Queen").Finalise(out QueenType);
            new UnitTypeCreator_TS(E, "King").Finalise(out KingType);

            AnyType = BoardManager.UnitRegistry.GetObjectTypeByID_TS("Any");

        }

        public void RegisterTurnBasedUnitTemplates(RegisterTurnBasedUnitTemplatesAnchor a) {
            Dictionary<string, GameObject> pieceTemplates = new Dictionary<string, GameObject>();

            foreach (ChessSettingsController.TemplateProfile profile in CommonSettingsController.BoardTemplates.TemplateProfiles)
                pieceTemplates.Add(profile.Name, profile.Template);

            Assert.True("There is a pawn template", pieceTemplates.ContainsKey("Pawn"));
            Assert.True("There is a rook template", pieceTemplates.ContainsKey("Rook"));
            Assert.True("There is a knight template", pieceTemplates.ContainsKey("Knight"));
            Assert.True("There is a bishop template", pieceTemplates.ContainsKey("Bishop"));
            Assert.True("There is a queen template", pieceTemplates.ContainsKey("Queen"));
            Assert.True("There is a king template", pieceTemplates.ContainsKey("King"));

            a.AddUnitTypeTemplate(LeftSide, "Pawn", pieceTemplates["Pawn"]);
            a.AddUnitTypeTemplate(LeftSide, "Rook", pieceTemplates["Rook"]);
            a.AddUnitTypeTemplate(LeftSide, "Knight", pieceTemplates["Knight"]);
            a.AddUnitTypeTemplate(LeftSide, "Bishop", pieceTemplates["Bishop"]);
            a.AddUnitTypeTemplate(LeftSide, "Queen", pieceTemplates["Queen"]);
            a.AddUnitTypeTemplate(LeftSide, "King", pieceTemplates["King"]);

            a.AddUnitTypeTemplate(RightSide, "Pawn", pieceTemplates["Pawn"]);
            a.AddUnitTypeTemplate(RightSide, "Rook", pieceTemplates["Rook"]);
            a.AddUnitTypeTemplate(RightSide, "Knight", pieceTemplates["Knight"]);
            a.AddUnitTypeTemplate(RightSide, "Bishop", pieceTemplates["Bishop"]);
            a.AddUnitTypeTemplate(RightSide, "Queen", pieceTemplates["Queen"]);
            a.AddUnitTypeTemplate(RightSide, "King", pieceTemplates["King"]);

        }

        public void RegisterTurnBasedGroupingUnitTypes(RegisterTurnBasedGroupingUnitTypesAnchor a) {

            //UnitObjectRegistry UnitRegistry = E.GetManager<BoardManager>().UnitRegistry;

            // Any Unit
            //UnitObjectTypeValue AnyUnitTypeVal = new UnitObjectTypeValue(E, "Any", CommonSettingsController.BoardTemplates.AnyUnitTemplate, UnitType.Get(E, (int) ChessPieceType.Any));
            //foreach (UnitObjectTypeValue type in UnitRegistry.AllObjectTypeValuesEnumerable_TS()) // Add all templates so that the Any type will be recgonised as any other type
            //    foreach (GameObject template in type.GetTemplates())
            //        if (!AnyUnitTypeVal.HasTemplate(template))
            //            AnyUnitTypeVal.AddTemplate(template);
            //RuleComponentManager.UnitObjectTypeByID.Add((int) ChessPieceType.Any, AnyUnitTypeVal);
            //RuleComponentManager.UnitObjectTypeByUnitTemplate.Add(AnyUnitTypeVal.GetTemplateRepresentative(), AnyUnitTypeVal);

            // Fill the UnitTemplateLookup with the values just added
            //foreach (UnitObjectTypeValue potv in RuleComponentManager.UnitObjectTypeByID.Values) {
            //    if (potv != AnyUnitTypeVal) {
            //        foreach (GameObject template in potv.GetTemplates()) {
            //            if (!RuleComponentManager.UnitObjectTypeByUnitTemplate.ContainsKey(template))
            //                E.GetManager<RuleComponentManager>().UnitObjectTypeByUnitTemplate.Add(template, potv);
            //        }
            //    }
            //}
        }

        public void RegisterTurnBasedPositionTypes(RegisterTurnBasedPositionTypesAnchor a) {

            new PositionTypeCreator_TS(E, "Square Center")
                .Finalise(out CenterPosition);

            new PositionTypeCreator_TS(E, "Square Edge")
                .SetNotInteractable()
                .Finalise(out EdgePosition);

            new PositionTypeCreator_TS(E, "Square Vertex")
                .SetNotInteractable()
                .Finalise(out VertexPosition);

            PositionType placemarker;
            new PositionTypeCreator_TS(E, "Square Edge Placemarker")
                .SetNotInteractable()
                .Finalise(out placemarker);
            BoardController.PositionTypePlacemarker = placemarker;

        }

        public void RegisterTurnBasedTileTypes(RegisterTurnBasedTileTypesAnchor a) {
            SquarePositionOffsets = new Vector3[9];

            List<Vector3> CenterDisplacements = new List<Vector3>();
            CenterDisplacements.Add(Vector3.up);
            CenterDisplacements.Add(-Vector3.up);
            CenterDisplacements.Add(Vector3.right);
            CenterDisplacements.Add(-Vector3.right);

            // Center position
            SquarePositionOffsets[(int) SquareTilePosition.Center] = Vector3.zero;

            // Edge positions
            SquarePositionOffsets[(int) SquareTilePosition.LeftEdge] = -0.5f * Vector3.right;
            SquarePositionOffsets[(int) SquareTilePosition.RightEdge] = 0.5f * Vector3.right;
            SquarePositionOffsets[(int) SquareTilePosition.TopEdge] = 0.5f * Vector3.up;
            SquarePositionOffsets[(int) SquareTilePosition.BottomEdge] = -0.5f * Vector3.up;

            // Vertex positions
            SquarePositionOffsets[(int) SquareTilePosition.TopLeftVertex] = 0.5f * Vector3.up - 0.5f * Vector3.right;
            SquarePositionOffsets[(int) SquareTilePosition.TopRightVertex] = 0.5f * Vector3.up + 0.5f * Vector3.right;
            SquarePositionOffsets[(int) SquareTilePosition.BottomLeftVertex] = -0.5f * Vector3.up - 0.5f * Vector3.right;
            SquarePositionOffsets[(int) SquareTilePosition.BottomRightVertex] = -0.5f * Vector3.up + 0.5f * Vector3.right;

            List<Vector3> SquareIslandNeighbourPattern = new List<Vector3>();
            SquareIslandNeighbourPattern.Add(0.5f * Vector3.right);
            SquareIslandNeighbourPattern.Add(-0.5f * Vector3.right);
            SquareIslandNeighbourPattern.Add(0.5f * Vector3.up);
            SquareIslandNeighbourPattern.Add(-0.5f * Vector3.up);

            foreach (ChessSettingsController.TileProfile profile in CommonSettingsController.BoardTemplates.TileTemplates) {
                TileType type;

                TileTypeCreator_TS tileTypeCreator = new TileTypeCreator_TS(E, profile.ID)
                    .AddTemplate(profile.Template)
                    .SetHeightRange(profile.MinimumHeight, profile.MaximumHeight)
                    .SetAllowRotation(profile.AllowRandomRotation)
                    .SetRarity(profile.Rarity);

                if (profile.Type == ChessTileType.SquareIsland) {

                    tileTypeCreator
                        .AddPositionType(SquarePositionOffsets[(int) SquareTilePosition.Center], CenterPosition)
                        .AddPositionType(SquarePositionOffsets[(int) SquareTilePosition.TopEdge], EdgePosition)
                        .AddPositionType(SquarePositionOffsets[(int) SquareTilePosition.LeftEdge], EdgePosition)
                        .AddPositionType(SquarePositionOffsets[(int) SquareTilePosition.BottomEdge], EdgePosition)
                        .AddPositionType(SquarePositionOffsets[(int) SquareTilePosition.RightEdge], EdgePosition)
                        .AddPositionType(SquarePositionOffsets[(int) SquareTilePosition.TopLeftVertex], VertexPosition)
                        .AddPositionType(SquarePositionOffsets[(int) SquareTilePosition.TopRightVertex], VertexPosition)
                        .AddPositionType(SquarePositionOffsets[(int) SquareTilePosition.BottomLeftVertex], VertexPosition)
                        .AddPositionType(SquarePositionOffsets[(int) SquareTilePosition.BottomRightVertex], VertexPosition)

                        .SetAllPositionNeighbours(SquareIslandNeighbourPattern)

                        .AddPositionDisplacements(SquarePositionOffsets[(int) SquareTilePosition.Center], CenterDisplacements);
                    
                } else {
                    Assert.Never("Tile type not supported: " + profile.Type);
                }

                tileTypeCreator.Finalise(out type);

            }

        }

        public void BuildTurnBasedBoard(BuildTurnBasedBoardAnchor a) {

            a.PlaceSquareGridOfTiles(new Vector2(1, 1), new Vector2(8, 8), a.GetRandomTileType, tiles);

        }

        public void RegisterTurnBasedUnits(RegisterTurnBasedUnitsAnchor a) {

            // Place units on board tiles
            for (float y = 1; y <= 8; y++) {
                new UnitCreator_TS(E, new Vector3(2, y), White, PawnType).Finalise();
                new UnitCreator_TS(E, new Vector3(7, y), Black, PawnType).Finalise();
            }

            new UnitCreator_TS(E, new Vector3(1, 1), White, RookType).Finalise();
            new UnitCreator_TS(E, new Vector3(8, 1), Black, RookType).Finalise();
            new UnitCreator_TS(E, new Vector3(1, 8), White, RookType).Finalise();
            new UnitCreator_TS(E, new Vector3(8, 8), Black, RookType).Finalise();

            new UnitCreator_TS(E, new Vector3(1, 2), White, KnightType).Finalise();
            new UnitCreator_TS(E, new Vector3(8, 2), Black, KnightType).Finalise();
            new UnitCreator_TS(E, new Vector3(1, 7), White, KnightType).Finalise();
            new UnitCreator_TS(E, new Vector3(8, 7), Black, KnightType).Finalise();

            new UnitCreator_TS(E, new Vector3(1, 3), White, BishopType).Finalise();
            new UnitCreator_TS(E, new Vector3(8, 3), Black, BishopType).Finalise();
            new UnitCreator_TS(E, new Vector3(1, 6), White, BishopType).Finalise();
            new UnitCreator_TS(E, new Vector3(8, 6), Black, BishopType).Finalise();

            new UnitCreator_TS(E, new Vector3(1, 4), White, QueenType).Finalise();
            new UnitCreator_TS(E, new Vector3(8, 4), Black, QueenType).Finalise();

            new UnitCreator_TS(E, new Vector3(1, 5), White, KingType).Finalise();
            new UnitCreator_TS(E, new Vector3(8, 5), Black, KingType).Finalise();

        }

        public void EffectInterfaceRegistration(EffectInterfaceRegistrationAnchor a) {
        }

        public void EffectImplementationRegistration(EffectImplementationRegistrationAnchor a) {
            a.RegisterEffectImplementation<IAddPositionEffect, AddPositionEffect>();
            a.RegisterEffectImplementation<IAddTileEffect, AddTileEffect>();
            a.RegisterEffectImplementation<IAddUnitEffect, AddUnitEffect>();
            a.RegisterEffectImplementation<IAnnouceLosersEffect, AnnouceLosersEffect>();
            a.RegisterEffectImplementation<IAnnouceWinnersEffect, AnnouceWinnersEffect>();
            a.RegisterEffectImplementation<IConfigureNewBoardObjectEffect, ConfigureNewBoardObjectEffect>();
            a.RegisterEffectImplementation<IDisplayMessageEffect, DisplayMessageEffect>();
            a.RegisterEffectImplementation<IHideNotificationEffect, HideNotificationEffect>();
            a.RegisterEffectImplementation<IInstantMoveBoardObjectEffect, InstantMoveBoardObjectEffect>();
            a.RegisterEffectImplementation<IMoveAndReturnUnitEffect, MoveAndReturnUnitEffect>();
            a.RegisterEffectImplementation<IMoveUnitEffect, MoveUnitEffect>();
            a.RegisterEffectImplementation<IRemovePositionEffect, RemovePositionEffect>();
            a.RegisterEffectImplementation<IRemoveTileEffect, RemoveTileEffect>();
            a.RegisterEffectImplementation<IRemoveUnitEffect, RemoveUnitEffect>();
            a.RegisterEffectImplementation<IShowNotificationEffect, ShowNotificationEffect>();
            a.RegisterEffectImplementation<IUpdateTileOfUnitEffect, UpdateTileOfUnitEffect>();
            a.RegisterEffectImplementation<IRuleExceptionEffect, RuleExceptionEffect>();
            a.RegisterEffectImplementation<IStartGameObjectPlacementEffect, StartGameObjectPlacementEffect>();
            a.RegisterEffectImplementation<IStopGameObjectPlacementEffect, StopGameObjectPlacementEffect>();
            a.RegisterEffectImplementation<IShowPossibleActionsForUnitEffect, ShowPossibleActionsForUnitEffect>();
            a.RegisterEffectImplementation<IHidePossibleActionsEffect, HidePossibleActionsEffect>();
            a.RegisterEffectImplementation<ITurnChangeEffect, TurnChangeEffect>();
        }

        public void BeforeStart(BeforeStartAnchor a) {

            // Register the two special unit variables for selection in collision rules
            RulePanelManager.RegisterSelectableVariable(TurnBasedExecutionManager.Collision, E.VariableManager.GetVariable_TS("Moving unit"));
            RulePanelManager.RegisterSelectableVariable(TurnBasedExecutionManager.Collision, E.VariableManager.GetVariable_TS("Target unit"));

        }

        public void RuleRegistration(RuleRegistrationAnchor a) {

            PawnTypeValue = BoardManager.UnitRegistry.GetObjectTypeValueByID_TS("Pawn");
            RookTypeValue = BoardManager.UnitRegistry.GetObjectTypeValueByID_TS("Rook");
            KnightTypeValue = BoardManager.UnitRegistry.GetObjectTypeValueByID_TS("Knight");
            BishopTypeValue = BoardManager.UnitRegistry.GetObjectTypeValueByID_TS("Bishop");
            QueenTypeValue = BoardManager.UnitRegistry.GetObjectTypeValueByID_TS("Queen");
            KingTypeValue = BoardManager.UnitRegistry.GetObjectTypeValueByID_TS("King");
            AnyTypeValue = BoardManager.UnitRegistry.GetObjectTypeValueByID_TS("Any");

            // Movement
            PawnMoveForwardOne();
            PawnMoveForwardTwoOnFirstTurn();
            PawnAttackSideways();
            RookMoves();
            KnightMoves();
            BishopMoves();
            QueenMoves();
            KingMoves();

            // Collision
            KingInCheckCancelsMove();
            CollisionRemovesTargetUnit();

            // Turn
            PawnBecomesQueen();
            KingInCheckAndNoUsableRulesGivesLoser();

            TestForPlacingTiles();

        }

        // Movement Rules
        // ==========

        // Pawns may move forward one as long as that place is not already occupied
        public void PawnMoveForwardOne() {


            Variable PawnVariable = new Variable(E, "Pawn", PawnTypeValue, null);

            new RuleCreator(E, MovementRuleType)
                .SetName("Pawn can move forward one, but not attack")
                .RegisterVariable(PawnVariable)
                .StartRule()
                    .AddIfStatement()
                        .SetAndConfigure<NotExpression>()
                            .SetAndConfigure<IsTileOccupiedRExpression>().Set(PawnVariable, new DirectionValue(E, Direction.Forward))
                        .ReturnTo<ExpressionCreator>()
                    .ReturnTo<IfStatementCreator>()
                        .StartBlock()
                            .AddAndConfigure<MoveStatement>().Set(PawnVariable, new DirectionValue(E, Direction.Forward))
                        .ReturnTo<BlockCreator>()
                    .EndBlock<IfStatementCreator>()
                .EndIfStatement<BaseBlockCreator>()
                .FinishRule()
                .Finalise();
            
        }

        // If x position is 2, the pawn may move two spaces forward
        public void PawnMoveForwardTwoOnFirstTurn() {

            Variable pawnV = new Variable(E, "Pawn", PawnTypeValue, null);
            
            new RuleCreator(E, MovementRuleType)
                .SetName("Pawn can move forward two on first turn")
                .RegisterVariable(pawnV)
                .StartRule()
                    .AddIfStatement()
                        .SetAndConfigure<AndExpression>()
                            .SetAndConfigure<EqualToExpression>()
                                .Set<GetYExpression>(pawnV)
                                .Set(E.IntegerObjectRegistry.CreateIntegerNumberValueFromInt_TS(2))
                        .ReturnTo<ExpressionCreator>()
                            .SetAndConfigure<NotExpression>().Set<IsTileOccupiedRExpression>(pawnV, new DirectionValue(E, new Vector3(0, 2)))
                        .ReturnTo<ExpressionCreator>()
                    .ReturnTo<IfStatementCreator>()
                        .StartBlock().Add<MoveStatement>(pawnV, new DirectionValue(E, new Vector3(0, 2)))
                    .EndBlock<IfStatementCreator>()
                .EndIfStatement<BaseBlockCreator>()
                .FinishRule()
                .Finalise();
            
        }

        // If there is a unit to the forward left or forward right, it may move there
        public void PawnAttackSideways() {

            // Forward left
            Variable pawnV = new Variable(E, "Pawn", PawnTypeValue, null);
            
            new RuleCreator(E, MovementRuleType)
                .SetName("Pawn can attack sideways")
                .RegisterVariable(pawnV)
                .StartRule()
                    .AddIfStatement()
                        .Set<IsTileOccupiedRExpression>(pawnV, new DirectionValue(E, Direction.Forward_left))
                        .StartBlock().Add<MoveStatement>(pawnV, new DirectionValue(E, Direction.Forward_left))
                    .EndBlock<IfStatementCreator>()
                .EndIfStatement<BaseBlockCreator>()
                .FinishRule()
                .Finalise();

            // Forward right
            pawnV = new Variable(E, "Pawn", PawnTypeValue, null);
            
            new RuleCreator(E, MovementRuleType)
                .SetName("Pawn can attack sideways")
                .RegisterVariable(pawnV)
                .StartRule()
                    .AddIfStatement()
                        .Set<IsTileOccupiedRExpression>(pawnV, new DirectionValue(E, Direction.Forward_right))
                        .StartBlock().Add<MoveStatement>(pawnV, new DirectionValue(E, Direction.Forward_right))
                    .EndBlock<IfStatementCreator>()
                .EndIfStatement<BaseBlockCreator>()
                .FinishRule()
                .Finalise();

        }

        public void RookMoves() {

            Variable rookV;
            
            foreach (Direction d in straightDirections) {

                rookV = new Variable(E, "Rook", RookTypeValue, null);
                
                new RuleCreator(E, MovementRuleType)
                    .SetName("The rook's moves")
                    .RegisterVariable(rookV)
                    .StartRule()
                        .AddPreLoopUntilStatement()
                            .Set<IsTileOccupiedRExpression>(rookV, new DirectionValue(E, Vector3.zero))
                            .StartBlock().Add<MoveStatement>(rookV, new DirectionValue(E, d))
                        .EndBlock<PreUntilLoopStatementCreator>()
                    .EndPreUntilLoopStatement<BaseBlockCreator>()
                    .Add<MoveStatement>(rookV, new DirectionValue(E, d))
                    .FinishRule()
                    .Finalise();

            }

        }

        public void KnightMoves() {
            
            new RuleCreator(E, MovementRuleType)
                .SetName("The knight's moves")
                .AddStatement(new MoveStatement(E, KnightTypeValue, new DirectionValue(E, new Vector3(1, 2))))
                .Finalise();

            new RuleCreator(E, MovementRuleType)
                .SetName("The knight's moves")
                .AddStatement(new MoveStatement(E, KnightTypeValue, new DirectionValue(E, new Vector3(1, -2))))
                .Finalise();

            new RuleCreator(E, MovementRuleType)
                .SetName("The knight's moves")
                .AddStatement(new MoveStatement(E, KnightTypeValue, new DirectionValue(E, new Vector3(-1, 2))))
                .Finalise();

            new RuleCreator(E, MovementRuleType)
                .SetName("The knight's moves")
                .AddStatement(new MoveStatement(E, KnightTypeValue, new DirectionValue(E, new Vector3(-1, -2))))
                .Finalise();

            new RuleCreator(E, MovementRuleType)
                .SetName("The knight's moves")
                .AddStatement(new MoveStatement(E, KnightTypeValue, new DirectionValue(E, new Vector3(2, 1))))
                .Finalise();

            new RuleCreator(E, MovementRuleType)
                .SetName("The knight's moves")
                .AddStatement(new MoveStatement(E, KnightTypeValue, new DirectionValue(E, new Vector3(2, -1))))
                .Finalise();

            new RuleCreator(E, MovementRuleType)
                .SetName("The knight's moves")
                .AddStatement(new MoveStatement(E, KnightTypeValue, new DirectionValue(E, new Vector3(-2, 1))))
                .Finalise();

            new RuleCreator(E, MovementRuleType)
                .SetName("The knight's moves")
                .AddStatement(new MoveStatement(E, KnightTypeValue, new DirectionValue(E, new Vector3(-2, -1))))
                .Finalise();

        }

        public void BishopMoves() {

            Variable bishopV;
            
            foreach (Direction d in diagonalDirections) {

                bishopV = new Variable(E, "Bishop", BishopTypeValue, null);
                
                new RuleCreator(E, MovementRuleType)
                    .SetName("The bishop's moves")
                    .RegisterVariable(bishopV)
                    .StartRule()
                        .AddPreLoopUntilStatement()
                            .Set<IsTileOccupiedRExpression>(bishopV, new DirectionValue(E, d))
                            .StartBlock().Add<MoveStatement>(bishopV, new DirectionValue(E, d))
                        .EndBlock<PreUntilLoopStatementCreator>()
                    .EndPreUntilLoopStatement<BaseBlockCreator>()
                    .Add<MoveStatement>(bishopV, new DirectionValue(E, d))
                    .FinishRule()
                    .Finalise();

            }

        }

        public void QueenMoves() {

            Variable queenV;

            foreach (Direction d in Enum.GetValues(typeof(Direction))) {

                queenV = new Variable(E, "Queen", QueenTypeValue, null);
                
                new RuleCreator(E, MovementRuleType)
                    .SetName("The queen's moves")
                    .RegisterVariable(queenV)
                    .StartRule()
                        .AddPreLoopUntilStatement()
                            .Set<IsTileOccupiedRExpression>(queenV, new DirectionValue(E, d))
                            .StartBlock().Add<MoveStatement>(queenV, new DirectionValue(E, d))
                        .EndBlock<PreUntilLoopStatementCreator>()
                    .EndPreUntilLoopStatement<BaseBlockCreator>()
                    .Add<MoveStatement>(queenV, new DirectionValue(E, d))
                    .FinishRule()
                    .Finalise();

            }

        }

        public void KingMoves() {

            Variable kingV;
            
            foreach (Direction d in Enum.GetValues(typeof(Direction))) {

                kingV = new Variable(E, "King", KingTypeValue, null);
                
                new RuleCreator(E, MovementRuleType)
                    .SetName("The king's moves")
                    .RegisterVariable(kingV)
                    .StartRule()
                        .Add<MoveStatement>(kingV, new DirectionValue(E, d))
                    .FinishRule()
                    .Finalise();

            }

        }

        // Collision rules
        // ===============

        public void KingInCheckCancelsMove() {

            Variable kingV = new Variable(E, "King", KingTypeValue, null);

            new RuleCreator(E, CollisionRuleType)
                .SetName("King in check cancels move")
                .RegisterVariable(kingV)
                .StartRule()
                    .AddIfStatement()
                        .SetAndConfigure<AndExpression>()
                            .Set<IsThreatenedExpression>(kingV)
                            .SetAndConfigure<IsCurrentPlayerExpression>().Set<PlayerOfUnitExpression>(kingV)
                        .ReturnTo<ExpressionCreator>()
                    .ReturnTo<IfStatementCreator>()
                        .StartBlock().Add<CancelMoveStatement>()
                    .EndBlock<IfStatementCreator>()
                .EndIfStatement<BaseBlockCreator>()
                .FinishRule()
                .Finalise();

        }

        // Collision rule to allow taking units of the other player
        public void CollisionRemovesTargetUnit() {
            
            new RuleCreator(E, CollisionRuleType)
                .SetName("One piece can take another")
                .StartRule()
                    .AddIfStatement()
                        .SetAndConfigure<NotExpression>()
                            .SetAndConfigure<IsCurrentPlayerExpression>()
                                .Set<PlayerOfUnitExpression>(E.VariableManager.GetVariable_TS("Target unit"))
                        .ReturnTo<ExpressionCreator>()
                    .ReturnTo<IfStatementCreator>()
                        .StartBlock().Add<RemoveUnitStatement>(E.VariableManager.GetVariable_TS("Target unit"))
                    .EndBlock<IfStatementCreator>()
                .EndIfStatement<BaseBlockCreator>()
                .FinishRule()
                .Finalise();

        }


        // Turn rules
        // ==========

        public void PawnBecomesQueen() {

            Variable pawnV = new Variable(E, "Pawn", PawnTypeValue, null);
            
            new RuleCreator(E, TurnRuleType)
                .SetName("Pawn can become Queen")
                .RegisterVariable(pawnV)
                .StartRule()
                    .AddIfStatement()
                        .SetAndConfigure<EqualToExpression>()
                            .Set<GetYExpression>(pawnV)
                            .Set(E.IntegerObjectRegistry.CreateIntegerNumberValueFromInt_TS(8))
                    .ReturnTo<IfStatementCreator>()
                        .StartBlock()
                            .Add<ReplaceUnitStatement>(pawnV, QueenTypeValue)
                    .EndBlock<IfStatementCreator>()
                .EndIfStatement<BaseBlockCreator>()
                .FinishRule()
                .Finalise();

        }

        // Prevents moves that allow a king to remain in check, and declares a winner ifno moves are possible for the king that is in check
        public void KingInCheckAndNoUsableRulesGivesLoser() {

            Variable kingV = new Variable(E, "King", KingTypeValue, null);
            
            new RuleCreator(E, TurnRuleType)
                .SetName("If you can't do anything, you lose")
                .RegisterVariable(kingV)
                .StartRule()
                    .AddIfStatement()
                        .SetAndConfigure<IsThreatenedExpression>().Set(kingV)
                    .ReturnTo<IfStatementCreator>()
                        .StartBlock()
                            .AddIfStatement()
                                .Set<NoRuleUsableExpression>()
                                .StartBlock()
                                    .AddAndConfigure<DeclareLoserStatement>()
                                        .SetAndConfigure<PlayerOfUnitExpression>().Set(kingV)
                                    .ReturnTo<StatementCreator>()
                                .ReturnTo<BlockCreator>()
                            .EndBlock<IfStatementCreator>()
                        .EndIfStatement<BlockCreator>()
                    .EndBlock<IfStatementCreator>()
                .EndIfStatement<BaseBlockCreator>()
                .FinishRule()
                .Finalise();

        }

        public void TestForPlacingTiles() {

            TileObjectTypeValue tileTypeValue = BoardManager.TileRegistry.GetObjectTypeValueByID_TS("Island1");

            new RuleCreator(E, TurnRuleType)
                .SetName("Place tile each turn")
                .AddStatement(new AddTileStatement(E, tileTypeValue))
                .Finalise();

        }

    }

}

