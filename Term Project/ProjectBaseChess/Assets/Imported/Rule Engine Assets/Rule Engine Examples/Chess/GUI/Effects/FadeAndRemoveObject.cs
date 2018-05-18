using RuleEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuleEngineExamples.Chess {

    class FadeAndRemoveObject : MonoBehaviour {

        private DateTime? RemovalTime = null;
        private List<Graphic> GraphicalComponents;

        public void SetFadeAndRemove(float duration) {

            GraphicalComponents = new List<Graphic>(GetComponentsInChildren<Graphic>());
            Assert.True("Graphic components were found in the children", GraphicalComponents.Count > 0);

            RemovalTime = DateTime.Now.AddSeconds(duration);

            foreach (Graphic g in GraphicalComponents) {
                g.CrossFadeAlpha(0, duration, false);
            }

        }
        
        void Update() {
            
            // Remove the object when it's time has expired
            if (RemovalTime.HasValue && DateTime.Now.CompareTo(RemovalTime.Value) >= 0)
                Destroy(gameObject);

        }

    }

}
