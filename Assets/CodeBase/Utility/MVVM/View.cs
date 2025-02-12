using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Assets.CodeBase.Utility.MVVM
{
	public abstract class View : MonoBehaviour, IInitializable, IDisposable
	{
		[SerializeField] private UIDocument _uiDocument;

		private UiPanel _currentPanel;
		private readonly Dictionary<Type, UiPanel> _panels = new();

		private VisualElement _contentPanel;

		public virtual void Initialize() {
			CacheContentPanel();
			InitializePanels();
			SubscribeToViewModel();
		}

		public virtual void Dispose() {
			DeactivateCurrentPanel();
			UnsubscribeFromViewModel();
			DisposePanels();
		}

		protected abstract void SubscribeToViewModel();

		protected abstract void UnsubscribeFromViewModel();

		protected void AddPanel<TPanel>(TPanel panel) where TPanel : UiPanel {
			_panels.Add(typeof(TPanel), panel);
		}

		protected void ActivatePanel<TPanel>() where TPanel : UiPanel {
			DeactivateCurrentPanel();
			EnablePanel<TPanel>();
			ShowCurrentPanel();
		}

		protected void DeactivateCurrentPanel() {
			HideCurrentPanel();
			DisableCurrentPanel();
		}

		private void CacheContentPanel() =>
			_contentPanel = _uiDocument.rootVisualElement
				.Q<VisualElement>(Constants.VisualElementNames.ContentPanel);

		private void InitializePanels() {
			foreach (UiPanel panel in _panels.Values)
				panel.Initialize();
		}

		private void DisposePanels() {
			foreach (UiPanel panel in _panels.Values)
				panel.Dispose();
		}

		private void EnablePanel<TPanel>() where TPanel : UiPanel {
			_currentPanel = _panels[typeof(TPanel)];
			_currentPanel.Enable();
		}

		private void DisableCurrentPanel() {
			if (_currentPanel == null)
				return;


			_currentPanel.Disable();
			_currentPanel = null;
		}

		private void ShowCurrentPanel() {
			_contentPanel.Add(_currentPanel.Panel);
		}

		private void HideCurrentPanel() {
			if (_contentPanel.childCount > 0)
				_contentPanel.RemoveAt(0);
		}
	}
}