using UnityEngine.EventSystems;

namespace UnityEngine.UI {
	
	[AddComponentMenu("Layout/Content Size Fitter Modified", 141)]	
	[ExecuteInEditMode]
	[RequireComponent (typeof (RectTransform))]
	public class ContentSizeFitterMod : UIBehaviour, ILayoutSelfController
	{
		public enum FitMode
		{
			Unconstrained,
			MinSize,
			PreferredSize
		}
		
		[SerializeField] public FitMode m_HorizontalFit = FitMode.PreferredSize;
		//public FitMode horizontalFit { get { return m_HorizontalFit; } set { SetPropertyUtility.SetStruct (ref m_HorizontalFit, value, SetDirty); } }
		
		[SerializeField] public FitMode m_VerticalFit = FitMode.PreferredSize;
		//public FitMode verticalFit { get { return m_VerticalFit; } set { SetPropertyUtility.SetStruct (ref m_VerticalFit, value,SetDirty); } }
		
		[System.NonSerialized] private RectTransform m_Rect;
		private RectTransform rectTransform
		{
			get
			{
				if (m_Rect == null)
					m_Rect = GetComponent<RectTransform> ();
				return m_Rect;
			}
		}

		private Rect cachedRect;
		public Rect GetRect() {
			return cachedRect;
		}
		public void Refresh() {
			SetDirty();
		}

		private DrivenRectTransformTracker m_Tracker;
		
		protected ContentSizeFitterMod()
		{}
		
		#region Unity Lifetime calls
		
		protected override void OnEnable ()
		{
			SetDirty ();
		}
		
		protected override void OnDisable()
		{
			m_Tracker.Clear ();
		}
		#endregion
		
		private void HandleSelfFittingAlongAxis (int axis)
		{
			FitMode fitting = (axis == 0 ? m_HorizontalFit : m_VerticalFit);
			if (fitting == FitMode.Unconstrained)
				return;
			
			m_Tracker.Add (this, rectTransform,
			               (axis == 0 ? DrivenTransformProperties.AnchorMaxX : DrivenTransformProperties.AnchorMaxY) |
			               (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));
			
			// Set anchor max to same as anchor min along axis
			Vector2 anchorMax = rectTransform.anchorMax;
			anchorMax[axis] = rectTransform.anchorMin[axis];
			rectTransform.anchorMax = anchorMax;
			
			// Set size to min size
			Vector2 sizeDelta = rectTransform.sizeDelta;
			if (fitting == FitMode.MinSize)
				sizeDelta[axis] = LayoutUtility.GetMinSize (m_Rect, axis);
			else
				sizeDelta[axis] = LayoutUtility.GetPreferredSize (m_Rect, axis);
			rectTransform.sizeDelta = sizeDelta;
			cachedRect = rectTransform.rect;
		}
		
		public void SetLayoutHorizontal ()
		{
			m_Tracker.Clear ();
			HandleSelfFittingAlongAxis (0);
		}
		
		public void SetLayoutVertical ()
		{
			HandleSelfFittingAlongAxis (1);
		}
		
		protected void SetDirty ()
		{
			if (!IsActive ())
				return;
			
			LayoutRebuilder.MarkLayoutForRebuild (rectTransform);
		}
		
		#if UNITY_EDITOR
		protected override void OnValidate ()
		{
			SetDirty ();
		}
		#endif
	}
	
}