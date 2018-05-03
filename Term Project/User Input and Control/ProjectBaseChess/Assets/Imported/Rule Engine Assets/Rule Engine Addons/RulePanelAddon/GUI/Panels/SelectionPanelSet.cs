using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {

    public class SelectionPanelSet {

		public GameObject indexPanel { get; private set; }
		public Dictionary<string, GameObject> selectionPanels { get; private set; }
		public GameObject selected;

		public SelectionPanelSet(GameObject indexPanel) {
			this.indexPanel = indexPanel;
			selectionPanels = new Dictionary<string, GameObject>();
		}

	}

}
