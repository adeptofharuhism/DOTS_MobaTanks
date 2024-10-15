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

        public static void AddUiPanel(this VisualElement element, UiPanel panel) {
            element.Add(panel.Panel);
            panel.Enable();
        }

        public static void RemoveUiPanel(this VisualElement element, UiPanel panel) {
            panel.Disable();
            element.Remove(panel.Panel);
        }
    }
}
