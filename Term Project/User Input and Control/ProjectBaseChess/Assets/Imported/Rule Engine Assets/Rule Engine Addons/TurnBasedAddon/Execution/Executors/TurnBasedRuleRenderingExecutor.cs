using RuleEngine;
using RuleEngineAddons.RulePanel;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public class TurnBasedRuleRenderingRuleExecutor : RuleRenderingExecutor, ITurnBasedRuleExecutor {

        public TurnBasedRuleRenderingRuleExecutor(Engine E, GameObject renderingPanel) : base (E, renderingPanel) {}

        public static void VisitDeclareWinnerStatement(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            if (v.PlayerManager.Players.Count == 1) {
                v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "You win");
            } else {
                v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "Declare");
                v.ArgumentAccept(obj, 0);
                v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "the winner", v.renderingPanel);
            }

        }

        public static void VisitDeclareLoserStatement(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            if (v.PlayerManager.Players.Count == 1) {
                v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "You lose");
            } else {
                v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "Declare");
                v.ArgumentAccept(obj, 0);
                v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "the loser", v.renderingPanel);
            }

        }

        public static void VisitMoveStatement(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.ArgumentAccept(obj, 0);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "can ", v.renderingPanel);
            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj);
            v.ArgumentAccept(obj, 1);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, ".", v.renderingPanel);

		}
		
		public static void VisitRemoveUnitStatement(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "(", v.renderingPanel);
            v.ArgumentAccept(obj, 0);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, ")", v.renderingPanel);
			
		}
		
		public static void VisitCancelMoveStatement(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "Cancel the move");
			
		}
		
		public static void VisitReplaceUnitStatement(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "Replace");
            v.ArgumentAccept(obj, 0);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "with", v.renderingPanel);
            v.ArgumentAccept(obj, 1);

		}

        public static void VisitAddTileStatement(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "Add ");
            v.ArgumentAccept(obj, 0);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "tile", v.renderingPanel);

        }


        
		public static void VisitGetXExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "parallel position of");
            v.ArgumentAccept(obj, 0);

		}
		
		public static void VisitGetYExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "forward position of");
            v.ArgumentAccept(obj, 0);

		}

		public static void VisitIsThreatenedExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.ArgumentAccept(obj, 0);
            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "is threatened");

            // Mark this rule as dependent on possible actions of unit if this expression is present
            v.CurrentRuleContext.Rule.PossibleActionDependent = true;

		}
        
		public static void VisitIsTileOccupiedRExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "Tile is occupied");
            v.ArgumentAccept(obj, 1);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "of", v.renderingPanel);
            v.ArgumentAccept(obj, 0);

		}
		
		public static void VisitPlayerOfUnitExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "player of unit ");
            v.ArgumentAccept(obj, 0);

		}
        
        public static void VisitIsCurrentPlayerExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.ArgumentAccept(obj, 0);
            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "is the current player");

		}

        public static void VisitIsPreviousPlayerExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.ArgumentAccept(obj, 0);
            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "is the previous player");

        }

        public static void VisitTypeOfUnitExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "type of");
            v.ArgumentAccept(obj, 0);

		}

        public static void VisitTypeOfTileExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "type of");
            v.ArgumentAccept(obj, 0);

        }

        public static void VisitTileOfUnitExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "tile of");
            v.ArgumentAccept(obj, 0);

        }

        public static void VisitGetCurrentPlayerExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "current player");

		}
		
		public static void VisitDirectionAdditionExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, "(", v.renderingPanel);
            v.ArgumentAccept(obj, 0);
            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "+");
            v.ArgumentAccept(obj, 1);
            v.PanelManager.WriteTextToPanel(panel, panel.TextTemplate, ")", v.renderingPanel);

		}

        public static void VisitNoRuleUsableExpression(RuleExecutor RuleExecutor, RuleComponent obj) {
            RuleRenderingExecutor v = (RuleRenderingExecutor) RuleExecutor;
            PanelDrawingAttachment panel = v.panel;
            v.CheckObject(obj);

            v.WriteClickableObjectToPanel(obj, panel.TextTemplate, obj, "No rule usable");

        }
        
    }

}

