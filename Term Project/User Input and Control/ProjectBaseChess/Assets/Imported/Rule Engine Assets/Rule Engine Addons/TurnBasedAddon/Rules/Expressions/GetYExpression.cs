using System;
using UnityEngine;
using RuleEngine;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class GetYExpression : Expression {

		public override Type GetReturnType() {
			return typeof(BooleanValue);
		}
        
		public override string GetSelectionPanelCategory() {
			return "Attributes";
		}

		public GetYExpression(Engine E) : base(E) {
			FillArgumentsWithNullValues();
		}
		
		public GetYExpression(Engine E, UnitObjectTypeValue p) : base(E) {
			ArgumentList.SetArgument(0, p.NewRef());
		}
		
		public GetYExpression(Engine E, Variable p) : base(E) {
			Assert.Same("Variable has type UnitObjectTypeValue", p.VariableType.GetType(), typeof(UnitObjectTypeValue));
			ArgumentList.SetArgument(0, p);
		}
        
		public override void DefineArguments() {
            ArgumentList.DefineArgument("Unit or type of unit", new Type[] { typeof(UnitObjectTypeValue), typeof(UnitObjectValue) }, "The unit or type unit of which to find the position.");
		}
        
		public override string GetDescription() {
			return "Gives the forward position of a unit, relative to a player.";
		}

        public override string ToString() {
            return "GetY";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            UnitObjectValue unit = argsByOrder[0].CalculateValue<UnitObjectValue>(RuleExecutor);

            Vector3 offset = unit.GetInstance().GetOffset_TS();
            Vector3 playerRel = unit.GetInstance().player.Position.GlobalToPlayerRelativePosition(offset);
            int y = (int) Math.Round((decimal) playerRel.y);

            return E.IntegerObjectRegistry.CreateIntegerNumberValueFromInt_TS(y);
        }
    }

}