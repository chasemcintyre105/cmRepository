using RuleEngineAddons.TurnBased;
using UnityEngine;

namespace RuleEngineAddons.RulePanel {

    public abstract class IMeshAttachment : MonoBehaviour {

        public abstract IBoardObjectAttachment GetAttachment();

    }

}