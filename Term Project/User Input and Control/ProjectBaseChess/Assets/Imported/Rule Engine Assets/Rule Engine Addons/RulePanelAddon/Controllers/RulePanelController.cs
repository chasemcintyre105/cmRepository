using RuleEngine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RuleEngineAddons.RulePanel {

    public class RulePanelController : IController {

        public override void Preinit() {

            TypeFunctions.CheckAllFieldsNotNull(GUITemplates);
            TypeFunctions.CheckAllFieldsNotNull(RulePanelDrawingBehaviourSettings);
            Assert.NotNull("OrganisationalObjects.Canvas", OrganisationalObjects.Canvas);
            Assert.NotNull("OrganisationalObjects.PanelsContainer", OrganisationalObjects.PanelsContainer);

        }

        public override void Init() {
        }

        [Serializable]
        public class OrganisationalObjectContainer {

            public Canvas Canvas;
            public GameObject PanelsContainer;
            public GameObject ActionStackContainer;

            // Generated at runtime
            [NonSerialized]
            public GameObject RulePanel;
            [NonSerialized]
            public GameObject BackdropPanel;
            [NonSerialized]
            public Scrollbar RulePanelScrollBar;
            [NonSerialized]
            public GameObject Tooltip;
            [NonSerialized]
            public Text TooltipText;
            [NonSerialized]
            public GameObject PGOTooltipContainer;

        }
        public OrganisationalObjectContainer OrganisationalObjects;

        [Serializable]
        public class GUITemplateContainer {
            public Text titleTextTemplate;
            public Text textTemplate;
            public Text textPlacemarkerTemplate;
            public GameObject borderedText;
            public GameObject panelTemplate;
            public Button buttonTemplate;
            public Button addButtonTemplate;
            public Button removeButtonTemplate;
            public GameObject ContainerTemplate;
            public GameObject BackdropTemplate;
            public Scrollbar ScrollBarTemplate;
            public GameObject tooltipTemplate;
            public GameObject tooltipPointerTemplate;
        }
        public GUITemplateContainer GUITemplates;

        [Serializable]
        public class RulePanelDrawingBehaviourSettingsContainer {
            public float LineSpacing = 10;
            public float PanelPadding = 20;
            public float TextPadding = 2;
            public float VerticalTextPadding = 5;
            public float HalfSpaceBetweenEditors = 10;
            public float StandardIndent = 50;
        }
        public RulePanelDrawingBehaviourSettingsContainer RulePanelDrawingBehaviourSettings;

        [Serializable]
        public class RuleJudgeObjectsContainer {
            
            public GameObject JudgementContainer;

            private string TempJudgementText = null;
            public string JudgementText {
                set {
                    if (_Judgement == null)
                        TempJudgementText = value;
                    else
                        _Judgement.text = value;
                }
            }
            private Text _Judgement;
            public Text Judgement {
                set {
                    _Judgement = value;

                    if (TempJudgementText != null)
                        _Judgement.text = TempJudgementText;
                }
            }

            public JudgeNotificationAttachment AddRule;
            public JudgeNotificationAttachment RemoveRule;
            public JudgeNotificationAttachment AddStatement;
            public JudgeNotificationAttachment RemoveObject;
            public JudgeNotificationAttachment SwapObjects;
            public JudgeNotificationAttachment ReplaceObject;
            public Button CancelButton;

            private PanelManager _PanelManager;
            private PanelManager PanelManager
            {
                get
                {
                    if (_PanelManager == null)
                        _PanelManager = RuleEngineController.E.GetManager<PanelManager>();

                    return _PanelManager;
                }
            }

        }
        public RuleJudgeObjectsContainer RuleJudgeObjects;

    }

}
