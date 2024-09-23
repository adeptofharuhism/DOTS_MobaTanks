using Assets.CodeBase.Infrastructure.StateMachine;
using Assets.CodeBase.Infrastructure.StateMachine.States;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Assets.CodeBase.UI
{
    public class ClientConnectionUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private VisualTreeAsset _connectionChoicePanel;
        [SerializeField] private VisualTreeAsset _joinGamePanel;
        [SerializeField] private VisualTreeAsset _hostGamePanel;

        private VisualElement _contentPanel;

        private VisualElement _connectionChoicePanelInstantiated;

        private VisualElement _joinGamePanelInstantiated;
        private TextField _joinPlayerName;
        private TextField _joinIPInput;
        private TextField _joinPortInput;

        private VisualElement _hostGamePanelInstantiated;
        private TextField _hostPlayerName;
        private TextField _hostPortInput;

        private IGameStateMachine _gameStateMachine;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine) {
            _gameStateMachine = gameStateMachine;
        }

        private void OnEnable() {
            InstantiatePanels();
            SetupContentPanel();
            SetupConnectionChoicePanel();
            SetupJoinGamePanel();
            SetupHostGamePanel();
        }

        private void InstantiatePanels() {
            _connectionChoicePanelInstantiated = InstantiatePanel(_connectionChoicePanel);
            _joinGamePanelInstantiated = InstantiatePanel(_joinGamePanel);
            _hostGamePanelInstantiated = InstantiatePanel(_hostGamePanel);
        }

        private VisualElement InstantiatePanel(VisualTreeAsset panelAsset) {
            VisualElement newPanel = panelAsset.Instantiate();
            newPanel.style.flexGrow = 1;

            return newPanel;
        }

        private void SetupContentPanel() {
            _contentPanel = _uiDocument.rootVisualElement.Q<VisualElement>(Constants.VisualElementNames.ConnectionMenu.ContentPanel);

            _contentPanel.Add(_connectionChoicePanelInstantiated);
        }

        private void SetupConnectionChoicePanel() {
            _connectionChoicePanelInstantiated
                .Q<Button>(Constants.VisualElementNames.ConnectionMenu.ConnectionChoicePanel.HostButton)
                .RegisterCallback<ClickEvent>(OnClickHostGame);
            _connectionChoicePanelInstantiated
                .Q<Button>(Constants.VisualElementNames.ConnectionMenu.ConnectionChoicePanel.ClientButton)
                .RegisterCallback<ClickEvent>(OnClickJoinGame);
            _connectionChoicePanelInstantiated
                .Q<Button>(Constants.VisualElementNames.ConnectionMenu.ConnectionChoicePanel.ExitButton)
                .RegisterCallback<ClickEvent>(OnClickExitGame);
        }

        private void SetupJoinGamePanel() {
            _joinIPInput = _joinGamePanelInstantiated.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.JoinIP);
            _joinPortInput = _joinGamePanelInstantiated.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.JoinPort);
            _joinPlayerName = _joinGamePanelInstantiated.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.PlayerName);

            _joinGamePanelInstantiated
                .Q<Button>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.JoinButton)
                .RegisterCallback<ClickEvent>(OnClickJoinAsClient);
            _joinGamePanelInstantiated
                .Q<Button>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.CancelButton)
                .RegisterCallback<ClickEvent>(OnClickCancel);
        }

        private void SetupHostGamePanel() {
            _hostPortInput = _hostGamePanelInstantiated.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.JoinPort);
            _hostPlayerName = _hostGamePanelInstantiated.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.PlayerName);

            _hostGamePanelInstantiated
                .Q<Button>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.HostButton)
                .RegisterCallback<ClickEvent>(OnClickHostButton);
            _hostGamePanelInstantiated
                .Q<Button>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.CancelButton)
                .RegisterCallback<ClickEvent>(OnClickCancel);
        }

        private void OnClickHostGame(ClickEvent evt) {
            ClearContentPanel();
            _contentPanel.Add(_hostGamePanelInstantiated);
        }

        private void OnClickJoinGame(ClickEvent evt) {
            ClearContentPanel();
            _contentPanel.Add(_joinGamePanelInstantiated);
        }

        private void OnClickExitGame(ClickEvent evt) =>
            Application.Quit();

        private void OnClickJoinAsClient(ClickEvent evt) =>
            _gameStateMachine.Enter<LoadMainSceneState, bool>(false);

        private void OnClickHostButton(ClickEvent evt) =>
            _gameStateMachine.Enter<LoadMainSceneState, bool>(true);

        private void OnClickCancel(ClickEvent evt) {
            ClearContentPanel();
            _contentPanel.Add(_connectionChoicePanelInstantiated);
        }

        private void ClearContentPanel() {
            if (_contentPanel.childCount > 0)
                _contentPanel.RemoveAt(0);
        }
    }
}
