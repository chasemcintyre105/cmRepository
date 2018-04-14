using UnityEngine;

namespace RuleEngineAddons.TurnBased {

    public interface IBoardObjectAttachment {

		void SetBoardObject(IBoardObject obj);
		IBoardObject GetBoardObject();
		bool HasBoardObject();
		GameObject GetGameObject();

        void OnMouseEnter();
        void OnMouseDown();
		void OnMouseUp();

	}

}
