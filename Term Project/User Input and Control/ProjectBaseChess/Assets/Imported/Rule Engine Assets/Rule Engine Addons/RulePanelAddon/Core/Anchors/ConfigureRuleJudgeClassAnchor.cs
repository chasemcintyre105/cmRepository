using RuleEngine;
using System;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {

    public class ConfigureRuleJudgeClassAnchor : Anchor {

        private RulePanelAddon RulePanelAddon;

        public ConfigureRuleJudgeClassAnchor(RuleEngineInitialiser initialiser, RulePanelAddon RulePanelAddon) : base(initialiser) {
            this.RulePanelAddon = RulePanelAddon;
        }

        public override void Init() {
        }

        public void SetRuleJudge(string ruleJudgeClassName) {
            RulePanelAddon.RuleJudgeClass = ruleJudgeClassName;
        }

        public void SetRuleJudge(Type ruleJudgeType) {
            RulePanelAddon.RuleJudgeClass = ruleJudgeType.FullName;
        }

        public void SetRuleJudge<RJ>() where RJ : RuleJudge {
            Type t = typeof(RJ);
            RulePanelAddon.RuleJudgeClass = t.FullName;
        }

        public override string GetDescription() {
            return "An anchor that allows for changing of the rule judge class.";
        }

    }

}