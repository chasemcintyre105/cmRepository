using RuleEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuleEngineAddons.RulePanel {

    public class SelectionPanelSetFactory {

		private Engine E;

		private SelectionPanelSet panelSet;
		private bool needsRemoveButton;

        private PanelManager _PanelManager;
        protected PanelManager PanelManager
        {
            get {
                if (_PanelManager == null)
                    _PanelManager = E.GetManager<PanelManager>();

                return _PanelManager;
            }
        }

        private RulePanelController _RulePanelController;
        protected RulePanelController RulePanelController
        {
            get
            {
                if (_RulePanelController == null)
                    _RulePanelController = E.GetController<RulePanelController>();

                return _RulePanelController;
            }
        }

        public SelectionPanelSetFactory(Engine E, RuleComponent obj, ArgumentAccessor parentAccessor, Rule containingRule, RuleType RuleType) {
			this.E = E;

            GameObject newPanel;
			Dictionary<string, List<RuleComponent>> options = E.GetManager<RulePanelManager>().GetSelectionOptions(obj, parentAccessor, containingRule, RuleType);

			panelSet = new SelectionPanelSet(NewPanel());
			AppendToListInPanel(panelSet.indexPanel, options.Keys);

			foreach (string id in options.Keys) {
				newPanel = NewPanel();
				AppendToListInPanel(E, newPanel, options[id]);
				panelSet.selectionPanels.Add(id, newPanel);
			}

			needsRemoveButton = E.RuleManager.RuleJudge.IsRemovingAnObjectAllowed() && !obj.Is<NullValue>() && !obj.Is<VoidStatement>();

		}

		public SelectionPanelSet Create(Rule currentRule) {

			if (needsRemoveButton)
				AppendRemoveStatementButtonToPanel(panelSet.indexPanel, currentRule);

            PanelManager PanelManager = E.GetManager<PanelManager>();

            PanelManager.SetPanelAsCenteredBestFit(panelSet.indexPanel, RulePanelController.OrganisationalObjects.Canvas);
			PanelManager.SetPanelVisibility(panelSet.indexPanel, false);

			foreach (GameObject panel in panelSet.selectionPanels.Values) {
				PanelManager.SetPanelAsCenteredBestFit(panel, RulePanelController.OrganisationalObjects.Canvas);
				PanelManager.SetPanelVisibility(panel, false);
			}

			return panelSet;
		}

		// For the object selection panels
		private void AppendToListInPanel(Engine E, GameObject panel, IEnumerable<RuleComponent> list) {
			GameObject newObject;
			Text text;
			
			foreach (RuleComponent ruleComponent in list) {
				newObject = E.GetManager<PanelManager>().NewInstance(RulePanelController.GUITemplates.buttonTemplate.gameObject, panel);
				text = newObject.GetComponentInChildren<Text>();
				Assert.NotNull("Text", text);
				text.text = ruleComponent.ToString();
				newObject.name = ruleComponent.ToString();
				Dictionary<string, object> properties = PanelManager.MakeGameObjectClickable(newObject, GUIEvent.Select_Object, false, null, null, null);
				properties.Add("Selection", ruleComponent);
				//newObject.AddComponent<StaticTooltipAttachment>().Tooltip = ruleComponent.GetTooltipDescription();
			}

		}

		// For the index panel
		private void AppendToListInPanel(GameObject panel, IEnumerable<string> list) {
			GameObject newObject;
			Text text;
			
			foreach (string categoryName in list) {
				newObject = E.GetManager<PanelManager>().NewInstance(RulePanelController.GUITemplates.buttonTemplate.gameObject, panel);
				text = newObject.GetComponentInChildren<Text>();
				Assert.NotNull("Text", text);
				text.text = categoryName;
				newObject.name = categoryName;
				Dictionary<string, object> properties = PanelManager.MakeGameObjectClickable(newObject, GUIEvent.Select_Category, false, null, null, null);
				properties.Add("Category", categoryName);
			}
			
		}

		private void AppendRemoveStatementButtonToPanel(GameObject panel, Rule currentRule) {

			GameObject newObject = E.GetManager<PanelManager>().NewInstance(RulePanelController.GUITemplates.buttonTemplate.gameObject, panel);
			Text text = newObject.GetComponentInChildren<Text>();
			Assert.NotNull("Text", text);
			text.text = "Remove";
			newObject.name = "Remove";
            Dictionary<string, object> properties = PanelManager.MakeGameObjectClickable(newObject, GUIEvent.Remove_Object, false, currentRule, null, null);
            properties.Add("Rule", currentRule);

		}

		private GameObject NewPanel() {
			return E.GetManager<PanelManager>().NewPanelInstance(RulePanelController.GUITemplates.panelTemplate, RulePanelController.OrganisationalObjects.PanelsContainer);
		}

	}

}
