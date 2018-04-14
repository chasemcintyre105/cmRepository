using RuleEngine;
using System;
using System.Collections.Generic;

namespace RuleEngineAddons.TurnBased {

    public class PlayerOfUnitExpression : Expression {

        public PlayerOfUnitExpression(Engine E) : base(E) {
            FillArgumentsWithNullValues();
        }

        public override void DefineArguments() {
            ArgumentList.DefineArgument("Unit or type of unit", new Type[] { typeof(UnitObjectTypeValue), typeof(UnitObjectValue) }, "The unit or unit type of which to determine the player.");
        }

        public void SetValue(Variable var) {
            Assert.Same("Type of variable is UnitObjectTypeValue", var.VariableType.GetType(), typeof(UnitObjectTypeValue));
            ArgumentList.SetArgument(0, var);
        }

        public void SetValue(UnitObjectTypeValue unitType) {
            ArgumentList.SetArgument(0, unitType);
        }

        public void SetValue(UnitObjectValue unit) {
            ArgumentList.SetArgument(0, unit);
        }

        public override Type GetReturnType() {
            return typeof(PlayerObjectValue);
        }
        
        public override string GetSelectionPanelCategory() {
            return "Attributes";
        }

        public override string GetDescription() {
            return "Gives the player of a unit.";
        }

        public override string ToString() {
            return "PlayerOfUnit";
        }

        public override RuleComponent CalculateExpression(RuleExecutor RuleExecutor, List<CREArgument> argsByOrder, Dictionary<string, CREArgument> argsByName) {
            UnitObjectValue argUnit = argsByOrder[0].CalculateValue<UnitObjectValue>(RuleExecutor);
            return new PlayerObjectValue(E, argUnit.GetInstance().player);
        }
    }

}