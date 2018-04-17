using System.Collections.Generic;
using RuleEngine;
using System;
using RuleEngineAddons.RulePanel;

namespace RuleEngineAddons.TurnBased {
	
	public class AddTileStatement : Statement {

        private TurnManager TurnManager;
        
        public override string GetSelectionPanelCategory() {
			return "Commands";
		}

		public AddTileStatement(Engine E) : base(E) {
			FillArgumentsWithNullValues();
            TurnManager = E.GetManager<TurnManager>();
        }

        public AddTileStatement(Engine E, TileObjectTypeValue tileType) : base(E) {
            ArgumentList.SetArgument(0, tileType);
        }
        
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Tile type", typeof(TileObjectTypeValue), "The type of the tile to add.", false);
		}

		public override string GetDescription() {
			return "Permits a player to add a new tile to the game this turn";
		}

        public override string ToString() {
            return "AddTile";
        }

        public override void ExecuteStatement(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            switch (RuleExecutor.RuleType.ID) {
            case TurnBasedExecutionManager.MOVEMENT:
                Assert.Never("Adding tiles not allowed in the Movement rules");
                break;
            case TurnBasedExecutionManager.COLLISION:
                Assert.Never("Adding tiles not allowed in the Collision rules");
                break;
            case TurnBasedExecutionManager.TURN:
                TileObjectTypeValue argTileType = argsByOrder[0].CalculateValue<TileObjectTypeValue>(RuleExecutor);
                
                if (E.HasAddon<RulePanelAddon>()) { 

                    Action EffectAction = null;
                    EffectAction = delegate () {
                        TurnManager = E.GetManager<TurnManager>();
                        if (TurnManager.TurnStateMachine.CurrentState == TurnState.Waiting) {
                            Dictionary<string, object> properties = new Dictionary<string, object>();
                            properties.Add("ObjectType", argTileType);
                            TurnManager.TurnStateMachine.IssueCommand(TurnEvent.Place_Object, properties);
                        } else {

                            E.GetManager<ModificationManager>().ApplyModification_TS(new AddStackObjectModification(E, "GameStack", new ActionStackItemProfile() {
                                ID = "PlaceTile",
                                Title = "Place " + argTileType.Name,
                                Type = ActionStackItemProfile.StackOptionType.Button,
                                Permanent = false,
                                DestroyOnClick = true,
                                OnClick = EffectAction
                            }));
                            
                        }

                    };

                    E.GetManager<ModificationManager>().ApplyModification_TS(new AddStackObjectModification(E, "GameStack", new ActionStackItemProfile() {
                        ID = "PlaceTile",
                        Title = "Place " + argTileType.Name,
                        Type = ActionStackItemProfile.StackOptionType.Button,
                        Permanent = false,
                        DestroyOnClick = true,
                        OnClick = EffectAction
                    }));

                }

                break;
            }
        }
    }

}