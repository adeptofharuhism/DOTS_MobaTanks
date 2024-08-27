using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets.UI_Sheets.Controls
{
    public class SquaredVisualElement : VisualElement
    {
        public const bool MatchHeightDefaultValue = true;

        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<SquaredVisualElement, UxmlTraits> { }

        [UnityEngine.Scripting.Preserve]
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlBoolAttributeDescription _matchHeight =
                new UxmlBoolAttributeDescription { name = "match-height", defaultValue = MatchHeightDefaultValue };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                ((SquaredVisualElement)ve).MatchHeight = _matchHeight.GetValueFromBag(bag, cc);
            }
        }

        public bool MatchHeight { get; private set; } = MatchHeightDefaultValue;

        public SquaredVisualElement() {
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanelEvent);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
        }

        private void OnAttachToPanelEvent(AttachToPanelEvent evt) {
            UpdateElement();
        }

        private void OnGeometryChangedEvent(GeometryChangedEvent evt) {
            UpdateElement();
        }

        private void UpdateElement() {
            if (MatchHeight)
                style.width = resolvedStyle.height;
            else
                style.height = resolvedStyle.width;
        }
    }
}
