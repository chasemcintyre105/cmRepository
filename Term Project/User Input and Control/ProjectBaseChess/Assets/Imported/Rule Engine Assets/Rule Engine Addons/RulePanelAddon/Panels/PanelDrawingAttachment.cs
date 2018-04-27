using RuleEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {

	public class PanelDrawingAttachment : MonoBehaviour {

        // Templates
        public GameObject TitleTextTemplate;
        public GameObject TextTemplate;
        public GameObject TextPlacemarkerTemplate;
        public GameObject AddButtonTemplate;
        public GameObject RemoveButtonTemplate;
        public GameObject ContainerTemplate;

        // Things that wont change after being set
        public float VerticalLineSpacing = 10f;
		public float PanelPadding = 10f;
		public float TextPadding = 5f;
		public float PanelWidth;
		public float HalfMaximumLineHeight;
		public float StandardIndent;
		
		// Things that will change with the contents of the panel
		[NonSerialized] public float HorizontalOffset;
		[NonSerialized] public float VerticalOffset;
		[NonSerialized] public float MaximumHeightOfCurrentLine;
		[NonSerialized] public float indent;
		[NonSerialized] public Stack<GameObject> ContainerStack;
		
		// Action restrictions
		[NonSerialized] public bool AddRuleRestricted = false;
		[NonSerialized] public bool AddStatementRestricted = false;
		[NonSerialized] public bool EditStatementRestricted = false;
		[NonSerialized] public bool EditExpressionRestricted = false;
		[NonSerialized] public bool EditValueRestricted = false;

		void Start() { 

			// Find PanelWidth
			RefreshPanelWidthAttribute();

		} 
		
		public void Reset() {
			
			// Calculate HalfMaximumLineHeight
			List<float> heights = new List<float>();
			heights.Add (((RectTransform) TextTemplate.transform).rect.height);
			heights.Add (((RectTransform) AddButtonTemplate.transform).rect.height);
			heights.Add (((RectTransform) RemoveButtonTemplate.transform).rect.height);
			HalfMaximumLineHeight = MaxFloat(heights) / 2;

			HorizontalOffset = PanelPadding;
			VerticalOffset = PanelPadding;
			MaximumHeightOfCurrentLine = 0f;
			ContainerStack = new Stack<GameObject>();
			ContainerStack.Push(gameObject);
			indent = 0;
		}

		public Canvas GetCanvas() {
			return GetComponentInParent<Canvas>();
		}

		public void RefreshPanelWidthAttribute() {
			RectTransform rectTransform = transform as RectTransform;
			Assert.NotNull("rectTransform", rectTransform);
			
			PanelWidth = rectTransform.rect.width;
		}

		private float MaxFloat(List<float> list) {
			if (list.Count == 0)
				return 0f; 
			if (list.Count == 1)
				return list[0];
			float max = list[0];
			for(int i = 1; i < list.Count; i++) {
				max = Math.Max(list[i-1], list[i]);
			}
			return max;
		}

	}

}

