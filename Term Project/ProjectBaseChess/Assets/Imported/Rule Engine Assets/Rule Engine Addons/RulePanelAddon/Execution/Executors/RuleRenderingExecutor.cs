using RuleEngine;
using RuleEngineAddons.TurnBased;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {

    public class RuleRenderingExecutor : RuleExecutor {

		public bool CurrentRuleInaccessible;
        public PanelManager PanelManager;
        public VariableManager VariableManager;
        public PlayerManager PlayerManager;

        public RuleRenderingExecutor(Engine E, GameObject renderingPanel) : base (E) {
			this.renderingPanel = renderingPanel;
            UseCompiledRules = false;
            PanelManager = E.GetManager<PanelManager>();
            VariableManager = E.GetManager<VariableManager>();
            PlayerManager = E.GetManager<PlayerManager>();
            panel = renderingPanel.GetComponent<PanelDrawingAttachment>();
			Assert.NotNull("PanelDrawingAttachment", panel);
		}

		public GameObject renderingPanel;
		public PanelDrawingAttachment panel;

		public override void Start() {

        }

		public override void Finish() {
            
		}

		public override void StartRule() {
            
            // Reset collision only flag
            CurrentRuleContext.Rule.CollisionOnly = false;

            // Reset the something editable flag
            CurrentRuleContext.Rule.SomeElementsEditable = false;

        }

        public override void FinishRule() {
            
		}


		// Statements
		public static void VisitBlock(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;

            v.CheckObject(obj);

            v.PanelManager.EnterBlock(panel);

			for (int i = 0; i < obj.ArgumentList.argsByOrder.Count; i++) {
                v.PanelManager.WriteNewLineToPanel(panel, false);
                v.ArgumentAccept(obj, i);

			}

			if (!v.CurrentRuleInaccessible && !panel.AddStatementRestricted && !v.CurrentRuleContext.Rule.Locked && v.E.RuleManager.RuleJudge.IsNewStatementAllowed()) {
                v.PanelManager.WriteNewLineToPanel(panel, false);
                Dictionary<string, object> properties = v.PanelManager.WriteButtonToPanel(panel, panel.AddButtonTemplate, GUIEvent.Add_Statement, null, null, null);
                properties.Add("Rule", RuleExecutor.CurrentRuleContext.Rule);
                properties.Add("Block", obj);
            }

            v.PanelManager.ExitBlock(panel);

		}

		public static void VisitIfStatement(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "If");
			v.AllArgumentAccept(obj);

		}
		
		public static void VisitIfElseStatement(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "If");
            v.ArgumentAccept(obj, 0);
            v.ArgumentAccept(obj, 1);
            v.PanelManager.WriteNewLineToPanel(panel, false);
            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "Otherwise");
            v.ArgumentAccept(obj, 2);

		}

        public static void VisitPreUntilLoopStatement(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            // No mention of max iterations for now
            // v.ArgumentAccept(obj, 1);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "Until");
            v.ArgumentAccept(obj, 0);
            v.PanelManager.WriteNewLineToPanel(panel, false);
            v.ArgumentAccept(obj, 2);

        }

        public static void VisitPostUntilLoopStatement(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            // No mention of max iterations for now
            // v.ArgumentAccept(obj, 1);

            v.ArgumentAccept(obj, 2);
            v.PanelManager.WriteNewLineToPanel(panel, false);
            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "Until");
            v.ArgumentAccept(obj, 0);

        }


        // Expressions
        public static void VisitEqualToExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "(", v.renderingPanel);
            v.ArgumentAccept(obj, 0);
            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "is equal to");
            v.ArgumentAccept(obj, 1);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, ")", v.renderingPanel);

		}

		public static void VisitNotEqualToExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "(", v.renderingPanel);
            v.ArgumentAccept(obj, 0);
            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "is not equal to");
            v.ArgumentAccept(obj, 1);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, ")", v.renderingPanel);
			
		}
        
		public static void VisitOrExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "(", v.renderingPanel);
            v.ArgumentAccept(obj, 0);
            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "or");
            v.ArgumentAccept(obj, 1);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, ")", v.renderingPanel);

		}
		
		public static void VisitAndExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "(", v.renderingPanel);
            v.ArgumentAccept(obj, 0);
            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "and");
            v.ArgumentAccept(obj, 1);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, ")", v.renderingPanel);

		}
		
		public static void VisitNotExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "(", v.renderingPanel);
            v.ArgumentAccept(obj, 0);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, ")", v.renderingPanel);

		}
        
        public void WriteClickableObjectToPanel(RuleComponent obj, GameObject Template, RuleComponent associatedObject) {
			WriteClickableObjectToPanel(obj, Template, associatedObject, obj.ToString());
		}

        public void WriteClickableObjectToPanel(RuleComponent obj, GameObject Template, RuleComponent associatedObject, string title) {
			bool isRestricted = false;

            if (E.RuleManager.RuleJudge.GetJudgement() == Judgement.Finished) {
                isRestricted = true;
            } else if(obj.Editability != RuleComponent.RuleComponentEditability.Not_Chosen) {
                if (obj.Editability == RuleComponent.RuleComponentEditability.Editable) {
                    isRestricted = false;
                } else if (obj.Editability == RuleComponent.RuleComponentEditability.Not_Editable) {
                    isRestricted = true;
                }
            } else  if (obj.Is<VoidStatement>() || obj.Is<NullValue>()) {
                isRestricted = false;
            } else {
                isRestricted =
                    CurrentRuleInaccessible ||
                    CurrentRuleContext.Rule.Locked ||
                    (obj.Is<Statement>() && panel.EditStatementRestricted) ||
                    (obj.Is<Expression>() && panel.EditExpressionRestricted) ||
                    (obj.Is<Value>() && panel.EditValueRestricted) ||
                    (obj.Is<Statement>() && E.RuleManager.RuleJudge.IsOnlyOneStatementAccessible() && E.RuleManager.RuleJudge.GetAccessibleRuleComponent() != obj.As<Statement>()) ||
                    (obj.Is<Value>() && !E.RuleManager.RuleJudge.IsRuleComponentAllowed(CurrentRuleContext.Rule, obj.As<Value>()));
            }

			if (isRestricted) {
				PanelManager.WriteTextToPanel(panel, Template, title, renderingPanel);
			} else {
                
                VisitedObject vo = visitedObjectsStack.Peek();
                ArgumentAccessor parentAccessor = new ArgumentAccessor(vo.obj, vo.index);
                Dictionary<string, object> props = PanelManager.WriteTextToPanel(panel,
                                                                                   Template,
                                                                                   title,
                                                                                   renderingPanel,
                                                                                   GUIEvent.Edit_Object,
                                                                                   false,
                                                                                   CurrentRuleContext.Rule,
				                                                                   associatedObject,
				                                                                   parentAccessor);
				props.Add("RuleType", RuleType);
                props.Add("RuleContext", CurrentRuleContext);
                props.Add("Object", obj);
				props.Add("ParentAccessor", parentAccessor);
			}
		}

        public void CheckObject(RuleComponent obj) {
            if (obj.Editability == RuleComponent.RuleComponentEditability.Editable)
                CurrentRuleContext.Rule.SomeElementsEditable = true;
        }


        
        public static void VisitVoidStatementOrNullValue(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            v.CheckObject(obj);
            v.WriteClickableObjectToPanel(obj, v.panel.TextPlacemarkerTemplate, obj, "Not selected");
        }

        public static void VisitVariable(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, v.panel.TextTemplate, obj);

            if (v.E.ExecutionManager is TurnBasedExecutionManager) { // Check if the addon is present 

                // If this rule is a collision rule and the variable is either Moving unit or Target unit, mark this rule as collision only
                if (v.RuleType == ((TurnBasedExecutionManager) v.E.ExecutionManager).Collision && (obj == v.VariableManager.GetVariable_TS("Moving unit") || obj == v.VariableManager.GetVariable_TS("Target unit"))) {
                    v.CurrentRuleContext.Rule.CollisionOnly = true;
                }

            }

        }
        
        public static void DefaultVisit(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj);
            if (obj.ArgumentList.argsByOrder.Count != 0) { 
                v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "(", v.renderingPanel);
                for (int i = 0; i < obj.ArgumentList.argsByOrder.Count; i++) {
                    if (i != 0)
                        v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, ", ", v.renderingPanel);
                    v.ArgumentAccept(obj, i);
                }
                v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, ")", v.renderingPanel);
            }

            if (obj is Statement)
                v.PanelManager.WriteNewLineToPanel(panel, false);

        }
        
    }

}

