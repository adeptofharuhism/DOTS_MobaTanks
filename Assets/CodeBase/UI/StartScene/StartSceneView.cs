using System;
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

        [Inject]
        public void Construct(IStartSceneViewModel startSceneViewModel) {
            _startSceneViewModel = startSceneViewModel;
        }

        public void Initialize() {

        }

        public void Dispose() {

        }
    }

    public abstract class UiPanel
    {
        public VisualElement Panel => _panel;

        protected readonly VisualElement _panel;

        public UiPanel(VisualTreeAsset panelAsset) {
            _panel = panelAsset.InstantiatePanel();

            CacheVisualElements();
        }

        public virtual void Enable() { }

        public virtual void Disable() { }

        protected virtual void CacheVisualElements() { }
    }

    public abstract class StartSceneUiPanel : UiPanel
    {
        protected readonly IStartSceneViewModel _startSceneViewModel;

        protected StartSceneUiPanel(VisualTreeAsset panelAsset, IStartSceneViewModel startSceneViewModel)
            : base(panelAsset) {

            _startSceneViewModel = startSceneViewModel;
        }
    }

    public class ConnectionChoicePanel : StartSceneUiPanel
    {
        private Button _hostButton;
        private Button _joinButton;
        private Button _exitButton;

        public ConnectionChoicePanel(VisualTreeAsset panelAsset, IStartSceneViewModel startSceneViewModel)
            : base(panelAsset, startSceneViewModel) { }

        public override void Enable() {
            _hostButton.RegisterCallback<ClickEvent>(OnClickHostButton);
            _joinButton.RegisterCallback<ClickEvent>(OnClickJoinButton);
            _exitButton.RegisterCallback<ClickEvent>(OnClickExitButton);
        }

        public override void Disable() {
            _hostButton.UnregisterCallback<ClickEvent>(OnClickHostButton);
            _joinButton.UnregisterCallback<ClickEvent>(OnClickJoinButton);
            _exitButton.UnregisterCallback<ClickEvent>(OnClickExitButton);
        }

        protected override void CacheVisualElements() {
            _hostButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.ConnectionChoicePanel.HostButton);
            _joinButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.ConnectionChoicePanel.JoinButton);
            _exitButton = _panel.Q<Button>(Constants.VisualElementNames.ConnectionMenu.ConnectionChoicePanel.ExitButton);
        }

        private void OnClickHostButton(ClickEvent evt) =>
            _startSceneViewModel.OnClickHostChoice();

        private void OnClickJoinButton(ClickEvent evt) =>
            _startSceneViewModel.OnClickHostChoice();

        private void OnClickExitButton(ClickEvent evt) =>
            _startSceneViewModel.OnClickExit();
    }

    public class HostPanel : StartSceneUiPanel
    {
        public HostPanel(VisualTreeAsset panelAsset, IStartSceneViewModel startSceneViewModel)
            : base(panelAsset, startSceneViewModel) { }


    }
}
