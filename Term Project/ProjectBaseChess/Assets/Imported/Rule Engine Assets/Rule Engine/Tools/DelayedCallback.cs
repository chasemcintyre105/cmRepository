using UnityEngine;
using System;

namespace RuleEngine {

	public class DelayedCallback : MonoBehaviour {
        
		public Action callback;

		public void SetTimedCallback(Action callback, float delayInSeconds) {
            this.callback = callback;
            Invoke("callbackFunction", delayInSeconds);
		}

        private void callbackFunction() {
            callback.Invoke();
            Destroy(this);
        }
        
	}

}