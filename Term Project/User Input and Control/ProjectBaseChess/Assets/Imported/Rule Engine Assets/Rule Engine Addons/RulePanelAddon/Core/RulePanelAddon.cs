using RuleEngine;
using System;

namespace RuleEngineAddons.RulePanel {

    public class RulePanelAddon : RuleEngineAddon {

        public string RuleJudgeClass = "RuleEngineAddons.RulePanel.DefaultRuleJudge";

        public PanelManager PanelManager;
        public RulePanelManager RulePanelManager;
        public ActionStackManager ActionStackManager;
        public NotificationManager NotificationManager;

        public override void Preinit() {
        }

        public override void Init() {
        }

        public override void RegisterAnchors() {
            RegisterAnchor(new ConfigureRuleJudgeClassAnchor(initialiser, this));
            RegisterAnchor(new ActionStackConfigurationAnchor(initialiser));
            RegisterAnchor(new RulePanelSelectableObjectRegistrationAnchor(initialiser));
        }

        public override void RegisterHooks() {
            RegisterHookWithAnchor<CheckDependenciesAnchor>(CheckDependencies);
            RegisterHookWithAnchor<ManagerRegistrationAnchor>(ManagerRegistration);
            RegisterHookWithAnchor<ManagerConfigurationAnchor>(ManagerConfiguration);
            RegisterHookWithAnchor<RuleExecutorRegistrationAnchor>(RuleExecutorRegistration);
            RegisterHookWithAnchor<RuleExecutorFunctionRegistrationAnchor>(RuleExecutorFunctionRegistration);
            RegisterHookWithAnchor<EffectInterfaceRegistrationAnchor>(EffectInterfaceRegistration);
            RegisterHookWithAnchor<EffectImplementationRegistrationAnchor>(EffectImplementationRegistration);
            RegisterHookWithAnchor<BeforeStartAnchor>(BeforeStart);
        }

        public void CheckDependencies(CheckDependenciesAnchor a) {
            a.RequireController<RulePanelController>();
        }

        public void ManagerRegistration(ManagerRegistrationAnchor a) {
            a.RegisterManager(new PanelManager(), delegate (IManager m) { PanelManager = (PanelManager) m; });
            a.RegisterManager(new RulePanelManager(), delegate (IManager m) { RulePanelManager = (RulePanelManager) m; });
            a.RegisterManager(new ActionStackManager(), delegate (IManager m) { ActionStackManager = (ActionStackManager) m; });
            a.RegisterManager(new NotificationManager(), delegate (IManager m) { NotificationManager = (NotificationManager) m; });
        }
        
        public void ManagerConfiguration(ManagerConfigurationAnchor a) {

            // Configure the GUI state machine
            PanelManager.GUIStateMachine = new StateMachine<GUIEvent, GUIState>(GUIState.Viewing_Rules, new GUITransitionHandler(E));
            PanelManager.GUIStateMachine.Init();
            PanelManager.GUIStateMachine
                .AddTransition(GUIState.Viewing_Game, GUIEvent.View_Rules, GUIState.Viewing_Rules) // Open the rule panel
                .AddTransition(GUIState.Viewing_Rules, GUIEvent.Check_Rules, GUIState.Checking_Rules) // Starting to close the rule panel

                .AddTransition(GUIState.Checking_Rules, GUIEvent.View_Rules, GUIState.Viewing_Rules) // Incomplete modifications to rules
                .AddTransition(GUIState.Checking_Rules, GUIEvent.Switch_Players, GUIState.Switching_Players) // Complete modifications to rules
                .AddTransition(GUIState.Checking_Rules, GUIEvent.View_Game, GUIState.Viewing_Game) // No modifications to rules

                .AddTransition(GUIState.Switching_Players, GUIEvent.View_Rules, GUIState.Viewing_Rules) // Cancel called in turn rules
                .AddTransition(GUIState.Switching_Players, GUIEvent.View_Game, GUIState.Viewing_Game) // All good, returning to the game

                // Editing the rules
                .AddTransition(GUIState.Viewing_Rules, GUIEvent.Remove_Rule, GUIState.Viewing_Rules)
                .AddTransition(GUIState.Viewing_Rules, GUIEvent.Add_Rule, GUIState.Viewing_Rules)
                .AddTransition(GUIState.Viewing_Rules, GUIEvent.Add_Statement, GUIState.Viewing_Rules)
                .AddTransition(GUIState.Viewing_Rules, GUIEvent.Edit_Object, GUIState.Selecting_Object_Category)
                .AddTransition(GUIState.Selecting_Object_Category, GUIEvent.Cancel_Selection, GUIState.Viewing_Rules)
                .AddTransition(GUIState.Selecting_Object_Category, GUIEvent.Select_Category, GUIState.Selecting_Object)
                .AddTransition(GUIState.Selecting_Object_Category, GUIEvent.Remove_Object, GUIState.Viewing_Rules)
                .AddTransition(GUIState.Selecting_Object, GUIEvent.Select_Object, GUIState.Viewing_Rules)
                .AddTransition(GUIState.Selecting_Object, GUIEvent.Cancel_Selection, GUIState.Viewing_Rules)
                .AddTransition(GUIState.Viewing_Rules, GUIEvent.Cancel_Changes, GUIState.Viewing_Rules)

                // Toggling the rules' visibility
                .AddTransition(GUIState.Viewing_Rules, GUIEvent.Toggle_Rule_Visible, GUIState.Viewing_Rules);

            // Get any changes to the class name
            ProcessAnchor<ConfigureRuleJudgeClassAnchor>();

            // Set up rule judge
            Assert.True("RuleJudge class is not blank", RuleJudgeClass != "");
            Type RuleJudgeType = Type.GetType(RuleJudgeClass);
            Assert.NotNull("RuleJudge class is valid: " + RuleJudgeClass, RuleJudgeType);
            Assert.True("Given RuleJudge type is a type of RuleJudge", RuleJudgeType.IsSubclassOf(typeof(RuleJudge)));
            
            // Set up new instance of the rule judge
            E.RuleManager.SetRuleJudge((RuleJudge) Activator.CreateInstance(RuleJudgeType, E));

        }

        public void RuleExecutorRegistration(RuleExecutorRegistrationAnchor a) {
            a.RegisterRuleExecutorType<RuleRenderingExecutor>();
        }

        public void RuleExecutorFunctionRegistration(RuleExecutorFunctionRegistrationAnchor a) {
            a.RegisterSpecificVisitFunction<Block, RuleRenderingExecutor>(RuleRenderingExecutor.VisitBlock);
            a.RegisterSpecificVisitFunction<IfStatement, RuleRenderingExecutor>(RuleRenderingExecutor.VisitIfStatement);
            a.RegisterSpecificVisitFunction<IfElseStatement, RuleRenderingExecutor>(RuleRenderingExecutor.VisitIfElseStatement);
            a.RegisterSpecificVisitFunction<PostUntilLoopStatement, RuleRenderingExecutor>(RuleRenderingExecutor.VisitPostUntilLoopStatement);
            a.RegisterSpecificVisitFunction<PreUntilLoopStatement, RuleRenderingExecutor>(RuleRenderingExecutor.VisitPreUntilLoopStatement);
            a.RegisterSpecificVisitFunction<EqualToExpression, RuleRenderingExecutor>(RuleRenderingExecutor.VisitEqualToExpression);
            a.RegisterSpecificVisitFunction<NotEqualToExpression, RuleRenderingExecutor>(RuleRenderingExecutor.VisitNotEqualToExpression);
            a.RegisterSpecificVisitFunction<AndExpression, RuleRenderingExecutor>(RuleRenderingExecutor.VisitAndExpression);
            a.RegisterSpecificVisitFunction<OrExpression, RuleRenderingExecutor>(RuleRenderingExecutor.VisitOrExpression);
            a.RegisterSpecificVisitFunction<NotExpression, RuleRenderingExecutor>(RuleRenderingExecutor.VisitNotExpression);

            a.RegisterSpecificVisitFunction<Variable, RuleRenderingExecutor>(RuleRenderingExecutor.VisitVariable);
            a.RegisterSpecificVisitFunction<VoidStatement, RuleRenderingExecutor>(RuleRenderingExecutor.VisitVoidStatementOrNullValue);
            a.RegisterSpecificVisitFunction<NullValue, RuleRenderingExecutor>(RuleRenderingExecutor.VisitVoidStatementOrNullValue);

            a.RegisterDefaultVisitFunction<RuleRenderingExecutor>(RuleRenderingExecutor.DefaultVisit);
        }
        
        public void EffectInterfaceRegistration(EffectInterfaceRegistrationAnchor a) {
            a.RegisterEffectInterface<IClosingRulePanelEffect>();
            a.RegisterEffectInterface<IOpeningRulePanelEffect>();
            a.RegisterEffectInterface<IShowNotificationEffect>();
            a.RegisterEffectInterface<IHideNotificationEffect>();
            a.RegisterEffectInterface<IShowPossibleActionsForUnitEffect>();
            a.RegisterEffectInterface<IHidePossibleActionsEffect>();
            a.RegisterEffectInterface<IAddStackObjectEffect>();
            a.RegisterEffectInterface<IRemoveStackObjectEffect>();
        }

        public void EffectImplementationRegistration(EffectImplementationRegistrationAnchor a) {
            a.RegisterEffectImplementation<IAddStackObjectEffect, AddStackObjectEffect>();
            a.RegisterEffectImplementation<IRemoveStackObjectEffect, RemoveStackObjectEffect>();
        }

        public void BeforeStart(BeforeStartAnchor a) {

            // Configure the action stack
            ActionStackManager.SetupInitialStackObjects();
            ProcessAnchor<ActionStackConfigurationAnchor>();

            // Statements
            RulePanelManager.RegisterSelectableObject(new IfStatement(E));
            RulePanelManager.RegisterSelectableObject(new IfElseStatement(E));
            RulePanelManager.RegisterSelectableObject(new PreUntilLoopStatement(E));
            RulePanelManager.RegisterSelectableObject(new PostUntilLoopStatement(E));

            // Expressions
            RulePanelManager.RegisterSelectableObject(new EqualToExpression(E));
            RulePanelManager.RegisterSelectableObject(new NotEqualToExpression(E));
            RulePanelManager.RegisterSelectableObject(new OrExpression(E));
            RulePanelManager.RegisterSelectableObject(new AndExpression(E));
            RulePanelManager.RegisterSelectableObject(new NotExpression(E));

            // Boolean values
            RulePanelManager.RegisterSelectableObject(new BooleanValue(E, true));
            RulePanelManager.RegisterSelectableObject(new BooleanValue(E, false));

            // Number values
            for (int i = 1; i <= 8; i++) {
                RulePanelManager.RegisterSelectableObject(E.IntegerObjectRegistry.CreateIntegerNumberValueFromInt_TS(i));
            }

            ProcessAnchor<RulePanelSelectableObjectRegistrationAnchor>();
            
            // First draw of all panels
            RulePanelManager.RedrawAllPanels();
            
            // Set the rule panels to their default position
            RulePanelManager.SetAllPanelsToDefaultPosition();

        }

    }

}

