using UnityEngine.UIElements;

namespace Assets.CodeBase.Utility.MVVM
{
    public static class UiExtensions
    {
        public static VisualElement InstantiatePanel(this VisualTreeAsset panelAsset) {
            VisualElement newPanel = panelAsset.Instantiate();
            newPanel.style.flexGrow = 1;

            return newPanel;
        }
    }
}
