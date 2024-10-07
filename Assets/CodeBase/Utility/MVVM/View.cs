using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Assets.CodeBase.Utility.MVVM
{
    public abstract class View : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private UIDocument _uiDocument;

        private Dictionary<Type, UiPanel> _panels = new Dictionary<Type, UiPanel>();
        private UiPanel _activePanel;

        private VisualElement _contentPanel;

        public virtual void Initialize() {
            InitializePanels();
            CacheContentPanel();
            SubscribeToViewModel();
        }

        public virtual void Dispose() {
            DeactivateActivePanel();
            UnsubscribeFromViewModel();
            DisposePanels();
        }

        protected abstract void SubscribeToViewModel();

        protected abstract void UnsubscribeFromViewModel();

        protected void AddPanel<TPanel>(TPanel panel) where TPanel : UiPanel {
            _panels.Add(typeof(TPanel), panel);
        }

        protected void ActivatePanel<TPanel>() where TPanel : UiPanel {
            DeactivateActivePanel();
            EnablePanel<TPanel>();
            ShowActivePanel();
        }

        protected void DeactivateActivePanel() {
            HideActivePanel();
            DisableActivePanel();
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
            _activePanel = _panels[typeof(TPanel)];
            _activePanel.Enable();
        }

        private void DisableActivePanel() {
            if (_activePanel == null)
                return;

            _activePanel.Disable();
            _activePanel = null;
        }

        private void ShowActivePanel() {
            _contentPanel.Add(_activePanel.Panel);
        }

        private void HideActivePanel() {
            if (_contentPanel.childCount > 0)
                _contentPanel.RemoveAt(0);
        }
    }
}
