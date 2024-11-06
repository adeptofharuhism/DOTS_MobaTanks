using UnityEngine.UIElements;

namespace Assets.CodeBase.Utility.MVVM
{
	public abstract class UiPart
	{
		protected readonly VisualElement _parent;

		public UiPart(VisualElement parent) {
			_parent = parent;
		}
		
		public void Initialize() {
			CacheVisualElements();
			ReadInitialViewModelData();
			BindData();
			RegisterCallbacks();
		}

		public void Dispose() {
			UnregisterCallbacks();
			UnbindData();
		}

		protected virtual void CacheVisualElements() { }
		protected virtual void ReadInitialViewModelData() { }
		protected virtual void BindData() { }
		protected virtual void RegisterCallbacks() { }

		protected virtual void UnregisterCallbacks() { }
		protected virtual void UnbindData() { }
	}
}