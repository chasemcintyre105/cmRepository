using RuleEngine;
using UnityEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewPreloadComponent : MonoBehaviour, IPreloadComponent {

        public void Init() {
            Debug.Log("Preload event");
        }

    }

}

