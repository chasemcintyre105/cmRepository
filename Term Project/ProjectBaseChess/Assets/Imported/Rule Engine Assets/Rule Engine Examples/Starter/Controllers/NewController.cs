using RuleEngine;
using System;
using UnityEngine;

namespace RuleEngineExamples.StarterProject {

    public class NewController : IController {
        
        public override void Preinit() {
        }

        public override void Init() {
        }

        [Serializable]
        public class TemplateContainer {
            public GameObject NewObjectTemplate;
        }
        public TemplateContainer Templates;

    }

}