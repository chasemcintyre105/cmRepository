using RuleEngine;
using UnityEngine;

namespace RuleEngineAddons.TurnBased {

	public interface IBoardObject : IObject {

		IBoardObjectType GetBoardObjectType();

		void SetOffset_TS(Vector3 offset);
		Vector3 GetOffset_TS();
		
		void SetBoardObjectAttachment(IBoardObjectAttachment obj);
		IBoardObjectAttachment GetBoardObjectAttachment();
		GameObject GetGameObject();

	}

}