using RuleEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuleEngineAddons.RulePanel {

    public class ActionStackProfile {

        private readonly object _lock = new object();

        private Engine E;
        private ActionStackManager ActionStackManager;

        public string ID { get; private set; }
        public GameObject Object { get; private set; }
        public RectTransform RectTransform { get; private set; }

        private List<ActionStackItemProfile> StackItems; // First is bottom, last is top
        private Dictionary<string, ActionStackItemProfile> StackItemsByID;

        public ActionStackProfile(Engine E, ActionStackManager ActionStackManager, string ID, string Name) {
            SelectiveDebug.LogActionStack("New ActionStackProfile: " + ID);

            this.E = E;
            this.ActionStackManager = ActionStackManager;
            this.ID = ID;

            StackItems = new List<ActionStackItemProfile>();
            StackItemsByID = new Dictionary<string, ActionStackItemProfile>();

            Object = new GameObject(Name);

            RegisterSelf();
            SetupRectTransform();
            InitStackObject();
        }

        public ActionStackProfile(Engine E, ActionStackManager ActionStackManager, string ID, string Name, ActionStackProfile ProfileToClone) {
            SelectiveDebug.LogActionStack("New ActionStackProfile: " + ID);

            this.E = E;
            this.ActionStackManager = ActionStackManager;
            this.ID = ID;

            StackItems = new List<ActionStackItemProfile>();
            StackItemsByID = new Dictionary<string, ActionStackItemProfile>();

            Object = GameObject.Instantiate<GameObject>(ProfileToClone.Object);
            Object.name = Name;

            RegisterSelf();
            SetupRectTransform();

            RectTransform.localPosition = Vector3.zero;
        }

        private void RegisterSelf() {
            Assert.False("ID is not yet registered: " + ID, ActionStackManager.Stacks.ContainsKey(ID));
            ActionStackManager.Stacks.Add(ID, this);
        }
        
        private void SetupRectTransform() {

            // Get (or add new) rect transform
            RectTransform = Object.GetComponent<RectTransform>();
            if (RectTransform == null)
                RectTransform = Object.AddComponent<RectTransform>();

            // Set the container of the stack
            RectTransform.SetParent(ActionStackManager.ActionStackContainer.transform);

            // Reset the scale of the object
            RectTransform.localScale = new Vector3(1, 1, 1);

        }

        private void InitStackObject() {

            // Further configuration of the stack
            RectTransform.anchorMin = Vector2.right;
            RectTransform.anchorMax = Vector2.right;
            ActionStackManager.ActionStackContainerRectTransform.pivot = ActionStackManager.AnchorTo;
            RectTransform.pivot = ActionStackManager.AnchorTo;
            RectTransform.localPosition = Vector3.zero;

            // Add and configure the grid layout component
            GridLayoutGroup GLG = Object.AddComponent<GridLayoutGroup>();
            GLG.cellSize = new Vector2(ActionStackManager.ItemWidth, ActionStackManager.ItemHeight);
            GLG.startCorner = GridLayoutGroup.Corner.LowerLeft;
            GLG.startAxis = GridLayoutGroup.Axis.Vertical;
            GLG.childAlignment = TextAnchor.LowerLeft;
            GLG.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            GLG.constraintCount = 1;
            GLG.spacing = new Vector2(0, ActionStackManager.ItemSpacing);

        }

        public void SetEditorName(string Name) {
            SelectiveDebug.LogActionStack(ToString() + " SetEditorName: " + Name);

            Object.name = Name;
        }

        public void SetActive(bool active) {
            SelectiveDebug.LogActionStack(ToString() + " SetActive: " + active);

            Object.SetActive(active);
        }

        public bool IsActive() {
            return Object.activeSelf;
        }

        public void AddItem_TS(ActionStackItemProfile Item, int? position = null) {
            SelectiveDebug.LogActionStack(ToString() + " AddItem_TS: " + Item.ID);

            lock (_lock) {
                Assert.False("Object does not already exist: " + Item.ID, StackItemsByID.ContainsKey(Item.ID));
                
                StackItemsByID.Add(Item.ID, Item);
                if (position.HasValue) {
                    StackItems.Insert(position.Value, Item);
                    E.EffectFactory.EnqueueNewEffect<IAddStackObjectEffect>(ID, Item, position);
                } else {
                    StackItems.Add(Item);
                    E.EffectFactory.EnqueueNewEffect<IAddStackObjectEffect>(ID, Item);
                }
            }
            
        }

        public void RemoveItem_TS(ActionStackItemProfile Item) {
            SelectiveDebug.LogActionStack(ToString() + " RemoveItem_TS");

            lock (_lock) {
                Assert.True("Object exists: " + Item.ID, StackItemsByID.ContainsKey(Item.ID));
                StackItems.Remove(StackItemsByID[Item.ID]);
                StackItemsByID.Remove(Item.ID);
                E.EffectFactory.EnqueueNewEffect<IRemoveStackObjectEffect>(ID, Item);
            }

        }

        public bool HasItem_TS(ActionStackItemProfile item) {
            SelectiveDebug.LogActionStack(ToString() + " HasItem_TS");

            lock (_lock) {
                return StackItems.Contains(item);
            }
        }

        public bool HasItemAtTop_TS(string StackItemID) {
            SelectiveDebug.LogActionStack(ToString() + " HasItemAtTop_TS");

            lock (_lock) {
                if (StackItems.Count == 0)
                    return false;

                return StackItems[StackItems.Count - 1].ID == StackItemID;
            }

        }

        public int GetPositionOfItem_TS(ActionStackItemProfile Item) {
            SelectiveDebug.LogActionStack(ToString() + " GetPositionOfItem_TS");

            lock (_lock) {
                return StackItems.IndexOf(Item);
            }

        }

        public GameObject GetGameObjectOfItem(string StackItemID) {
            SelectiveDebug.LogActionStack(ToString() + " GetGameObjectOfItem " + StackItemID);

            return StackItemsByID[StackItemID].Object;
        }

        public void ForEachTemporaryItem_TS(Action<ActionStackItemProfile> callback) {
            SelectiveDebug.LogActionStack(ToString() + " ForEachTemporaryItem_TS");

            lock (_lock) {
                foreach (ActionStackItemProfile Item in StackItems) {
                    if (!Item.Permanent)
                        callback(Item);
                }
            }
        }

        public GameObject AddButtonItem(ActionStackItemProfile Item) {
            SelectiveDebug.LogActionStack(ToString() + " AddButtonItem: " + Item.ID);
            
            GameObject newButton = AddLabelItem(Item);

            Button button = newButton.GetComponent<Button>();

            // Clear any destroy on click action set previously if necessary
            if (!Item.DestroyOnClick)
                button.onClick.RemoveAllListeners();

            // Set the callback function if present
            if (Item.OnClick != null)
                button.onClick.AddListener(() => Item.OnClick.Invoke());

            return newButton;
        }

        public GameObject AddLabelItem(ActionStackItemProfile Item) {
            SelectiveDebug.LogActionStack(ToString() + " AddLabelItem: " + ID);

            // Create the label and put it into the item
            GameObject newLabel = new GameObject(Item.ID);
            Item.Object = newLabel;

            // Set background color for text
            Image image = newLabel.AddComponent<Image>();
            image.color = new Color(1, 1, 1, 0.2f);

            // Configure sub object for text
            GameObject newText = new GameObject("Text");
            newText.transform.SetParent(newLabel.transform);

            Text text = newText.AddComponent<Text>();
            text.text = Item.Title;
            text.color = new Color(0.2f, 0.2f, 0.2f);
            text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = new Vector2(1, 1);
            text.transform.localPosition = Vector2.zero;
            text.rectTransform.sizeDelta = Vector2.zero;
            text.alignment = TextAnchor.MiddleCenter;
            
            // Set click action
            Button button = newLabel.AddComponent<Button>();
            if (Item.DestroyOnClick) { 
                button.onClick.AddListener(delegate {
                    E.GetManager<ModificationManager>().ApplyModification_TS(new RemoveStackObjectModification(E, ID, Item));
                });
            }

            return newLabel;
        }
        
        public void AddGameObjectAtTheTop(ActionStackItemProfile item) {
            SelectiveDebug.LogActionStack("AddGameObjectAtTheTop: " + item.Title);

            item.Object.transform.SetParent(RectTransform);
            item.Object.transform.SetAsLastSibling();
            item.Object.transform.localScale = new Vector3(1, 1, 1);

        }

        public void AddGameObjectAtPosition(ActionStackItemProfile item, int position) {
            SelectiveDebug.LogActionStack("AddGameObjectAtPosition: " + item.Title + ", " + position);

            item.Object.transform.SetParent(RectTransform);
            item.Object.transform.SetSiblingIndex(position);
            item.Object.transform.localScale = new Vector3(1, 1, 1);

        }

        public void AddGameObjectAtTheBottom(ActionStackItemProfile item) {
            SelectiveDebug.LogActionStack("AddGameObjectAtTheBottom: " + item.Title);

            item.Object.transform.SetParent(RectTransform);
            item.Object.transform.SetAsFirstSibling();
            item.Object.transform.localScale = new Vector3(1, 1, 1);

        }
        
        public void UpdateStackDimensions() {
            RectTransform.sizeDelta = new Vector2(ActionStackManager.ItemWidth,
                                                  RectTransform.childCount * ActionStackManager.ItemHeight + (RectTransform.childCount - 1) * ActionStackManager.ItemSpacing);
        }
        
        public override string ToString() {
            return ID;
        }

    }

}
