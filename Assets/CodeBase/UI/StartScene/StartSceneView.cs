using Assets.CodeBase.UI.StartScene.Panels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Assets.CodeBase.UI.StartScene
{
    public class StartSceneView : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private VisualTreeAsset _connectionChoicePanel;
        [SerializeField] private VisualTreeAsset _joinGamePanel;
        [SerializeField] private VisualTreeAsset _hostGamePanel;

        private IStartSceneViewModel _startSceneViewModel;

        private Dictionary<Type, UiPanel> _panels;
        private UiPanel _activePanel;

        private VisualElement _contentPanel;

        [Inject]
        public void Construct(IStartSceneViewModel startSceneViewModel) {
            _startSceneViewModel = startSceneViewModel;

            _panels = new Dictionary<Type, UiPanel> {
                [typeof(ConnectionChoicePanel)] = new ConnectionChoicePanel(_connectionChoicePanel, startSceneViewModel),
                [typeof(JoinPanel)] = new JoinPanel(_joinGamePanel, startSceneViewModel),
                [typeof(HostPanel)] = new HostPanel(_hostGamePanel, startSceneViewModel),
            };
        }

        public void Initialize() {
            InitializePanels();

            CacheContentPanel();
            SubscribeToViewModel();
            ActivatePanel<ConnectionChoicePanel>();
        }

        public void Dispose() {
            DeactivateActivePanel();
            UnsubscribeFromViewModel();

            DisposePanels();
        }

        private void InitializePanels() {
            foreach (UiPanel panel in _panels.Values)
                panel.Initialize();
        }

        private void DisposePanels() {
            foreach (UiPanel panel in _panels.Values)
                panel.Dispose();
        }

        private void CacheContentPanel() =>
            _contentPanel = _uiDocument.rootVisualElement
                .Q<VisualElement>(Constants.VisualElementNames.ConnectionMenu.ContentPanel);

        private void SubscribeToViewModel() =>
            _startSceneViewModel.Mode.OnChanged += OnChangeMode;

        private void UnsubscribeFromViewModel() =>
            _startSceneViewModel.Mode.OnChanged -= OnChangeMode;

        private void ActivatePanel<TPanel>() where TPanel : UiPanel {
            DeactivateActivePanel();
            EnablePanel<TPanel>();
            ShowActivePanel();
        }

        private void DeactivateActivePanel() {
            HideActivePanel();
            DisableActivePanel();
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

        private void OnChangeMode(StartSceneMode mode) {
            switch (mode) {
                case StartSceneMode.Join:
                    ActivatePanel<JoinPanel>();
                    break;
                case StartSceneMode.Host:
                    ActivatePanel<HostPanel>();
                    break;
                case StartSceneMode.ConnectionChoice:
                default:
                    ActivatePanel<ConnectionChoicePanel>();
                    break;
            }
        }
    }
}
