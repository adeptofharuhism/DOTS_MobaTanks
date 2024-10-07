using UnityEngine.UIElements;

namespace Assets.CodeBase.Utility.MVVM
{
    public abstract class UiPanel
    {
        public VisualElement Panel => _panel;

        protected readonly VisualElement _panel;

        protected UiPanel(VisualTreeAsset panelAsset) {
            _panel = panelAsset.InstantiatePanel();

            CacheVisualElements();
        }

        public virtual void Enable() { }

        public virtual void Disable() { }

        public void Initialize() {
            ReadInitialViewModelData();
            BindData();
        }

        public void Dispose() {
            UnbindData();
        }

        protected virtual void ReadInitialViewModelData() { }

        protected virtual void BindData() { }

        protected virtual void UnbindData() { }

        protected virtual void CacheVisualElements() { }
    }
}
