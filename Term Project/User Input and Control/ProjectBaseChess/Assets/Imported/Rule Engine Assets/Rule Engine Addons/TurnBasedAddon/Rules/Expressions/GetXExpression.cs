using System;
using UnityEngine;
using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class GetXExpression : Expression {

		public override Type GetReturnType() {
			return typeof(BooleanValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Attributes";
		}

		public GetXExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public GetXExpression(Engine E, UnitObjectTypeValue p) : base(E) {
			ArgumentList.SetArgument(0, p.NewRef());
		}
		
		public GetXExpression(Engine E, Variable p) : base(E) {
			Assert.Same("Variable has type UnitObjectTypeValue", p.VariableType.GetType(), typeof(UnitObjectTypeValue));
			ArgumentList.SetArgument(0, p);
		}
        
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Unit or type of unit", new Type[] { typeof(UnitObjectTypeValue), typeof(UnitObjectValue) }, "The unit or type unit of which to find the position.");
		}

		public override string GetDescription() {
			return "Gives the sideways position of a unit, relative to a player.";
		}

        public override string ToString() {
            return "GetX";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            UnitObjectValue unit = argsByOrder[0].CalculateValue<UnitObjectValue>(RuleExecutor);

            Vector3 offset = unit.GetInstance().GetOffset_TS();
            Vector3 playerRel = unit.GetInstance().player.Position.GlobalToPlayerRelativePosition(offset);
            int x = (int) Math.Round((decimal) playerRel.x);
            
            return E.IntegerObjectRegistry.CreateIntegerNumberValueFromInt_TS(x);
        }
    }

}