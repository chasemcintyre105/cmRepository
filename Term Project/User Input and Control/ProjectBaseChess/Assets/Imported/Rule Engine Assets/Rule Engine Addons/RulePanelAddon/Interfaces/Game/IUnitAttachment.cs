using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {

    public abstract class IUnitAttachment : MonoBehaviour, IBoardObjectAttachment, DragAndDropActionable {
        public abstract IBoardObject GetBoardObject();
        public abstract GameObject GetGameObject();
        public abstract bool HasBoardObject();
        public abstract void Hover();
        public abstract void OnMouseDown();
        public abstract void OnMouseEnter();
        public abstract void OnMouseUp();
        public abstract void PossibleAction();
        public abstract void SetBoardObject(IBoardObject obj);
        public abstract void Unhover();
        public abstract void UnpossibleAction();
    }
}