using UnityEngine.UIElements;

namespace Assets.CodeBase.Utility.MVVM
{
	public abstract class UiPanel
	{
		public VisualElement Panel => _panel;

		protected readonly VisualElement _panel;

		protected UiPanel(VisualTreeAsset panelAsset) {
			_panel = panelAsset.InstantiatePanel();
		}

		public void Enable() {
			_panel.SetEnabled(true);
		}

		public void Disable() {
			_panel.SetEnabled(false);
		}

		public void Initialize() {
			CacheVisualElements();
			InitializeSubPanels();
			ReadInitialViewModelData();
			BindData();
			RegisterCallbacks();
		}

		public void Dispose() {
			UnregisterCallbacks();
			UnbindData();
			DisposeSubPanels();
		}

		protected virtual void CacheVisualElements() { }
		
		protected virtual void InitializeSubPanels(){}

		protected virtual void ReadInitialViewModelData() { }

		protected virtual void RegisterCallbacks() { }
		protected virtual void UnregisterCallbacks() { }

		protected virtual void BindData() { }

		protected virtual void UnbindData() { }
		
		protected virtual void DisposeSubPanels(){}
	}
}