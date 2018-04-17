using RuleEngine;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {

    public class SelectionPanelManager {

		private GameObject CurrentPanel;
		private SelectionPanelSet CurrentPanelSet;

        private PanelManager PanelManager;
        private RulePanelController RulePanelController;

        public void SetEngine(Engine E) {
            PanelManager = E.GetManager<PanelManager>();
            RulePanelController = E.GetController<RulePanelController>();
        }

		// Used to display a single selection panel
		public void DisplaySelectionPanel(GameObject panel) {
			Assert.Null("Current panel", CurrentPanel);
			Assert.Null("Current panel set", CurrentPanelSet);

			CurrentPanel = panel;
			PanelManager.SetAsPopUpWithBackdrop(panel, RulePanelController.OrganisationalObjects.BackdropPanel);

		}

		// Used to display and re-display the index panel
		public void DisplaySelectionPanelSetIndex(SelectionPanelSet panelSet) {
			Assert.Null("Current panel", CurrentPanel);

			CurrentPanelSet = panelSet;
			if (CurrentPanelSet.selected != null)
				PanelManager.SetPanelVisibility(CurrentPanelSet.selected, false);
			PanelManager.SetAsPopUpWithBackdrop(CurrentPanelSet.indexPanel, RulePanelController.OrganisationalObjects.BackdropPanel);
			CurrentPanelSet.selected = CurrentPanelSet.indexPanel;

		}

		// Use to display the selection panel for a specific category of objects
		public void DisplaySelectionPanelSetCategory(string category) {
			Assert.NotNull("Current panel set", CurrentPanelSet);
			Assert.True("Index is current panel being shown", CurrentPanelSet.selected == CurrentPanelSet.indexPanel);
			Assert.True("Panel set contains the category", CurrentPanelSet.selectionPanels.ContainsKey(category));

			PanelManager.SetPanelVisibility(CurrentPanelSet.indexPanel, false);

			GameObject categoryPanel = CurrentPanelSet.selectionPanels[category];
			PanelManager.SetAsPopUpWithBackdrop(categoryPanel, RulePanelController.OrganisationalObjects.BackdropPanel);
			CurrentPanelSet.selected = categoryPanel;

		}

		public void TurnOffSelectionPanel() {

			if (CurrentPanel != null) {

				PanelManager.SetPanelVisibility(CurrentPanel, false);
				PanelManager.SetPanelVisibility(RulePanelController.OrganisationalObjects.BackdropPanel, false);
				CurrentPanel = null;

			} else if (CurrentPanelSet != null) {

				PanelManager.SetPanelVisibility(CurrentPanelSet.selected, false);
				PanelManager.SetPanelVisibility(RulePanelController.OrganisationalObjects.BackdropPanel, false);
				CurrentPanelSet = null;

			} else
				Assert.Never("Either current panel or current panel set must be not null");

		}

	}

}