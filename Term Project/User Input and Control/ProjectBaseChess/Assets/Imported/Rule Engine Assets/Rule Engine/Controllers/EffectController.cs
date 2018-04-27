using System;
using System.Collections.Generic;

namespace RuleEngine {

    public class EffectController : IController {
        
		private readonly object _queueLock = new object();

		private Effect effect;
		private Queue<Effect> localEffectsCopy;
		private Queue<Effect> tmp;

        public override void Preinit() {
            localEffectsCopy = new Queue<Effect>();
        }

        public override void Init() {
        }

        void Update () {

			// Check the effect queue for new effects to do
			lock (_queueLock) {
                if (EffectQueue.Count > 0) { 
                    tmp = EffectQueue;
                    EffectQueue = localEffectsCopy;
                    localEffectsCopy = tmp;
                }
            }
			
			// Move the application of the effects outside of the lock to not hog the lock and cause potential problems
			// Do this via an extra queue to preserve order and avoid repeated re-entry to the lock
			while (localEffectsCopy.Count > 0) {
                localEffectsCopy.Dequeue().Apply();
            }

		}
		
		// Effect management
		
		// Queues and counters for use in thread safe regions of code
		private Queue<Effect> EffectQueue = new Queue<Effect>();

		public void EnqueueEffect_TS(Effect newEffect) {
			lock (_queueLock) {
				EffectQueue.Enqueue(newEffect);
			}
		}
        

        // Time delayed function execution
        public void InvokeWithDelay(Action callback, float delayInSeconds) {
            gameObject.AddComponent<DelayedCallback>().SetTimedCallback(callback, delayInSeconds);
        }

        public void EnqueueEffectWithDelay_TS(Effect effect, float delayInSeconds) {
            gameObject.AddComponent<DelayedCallback>().SetTimedCallback(delegate () {
                // Called in the main thread
                effect.Apply();
            }, delayInSeconds);
        }

    }

}
