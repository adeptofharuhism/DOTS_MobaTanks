using System;
using UnityEngine;
using UnityEngine.UIElements;

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
        private TextField _joinIPInput;
        private TextField _joinPortInput;
        
        private VisualElement _hostGamePanelInstantiated;
        private TextField _hostIPInput;
        private TextField _hostPortInput;

        private void OnEnable() {
            InstantiatePanels();
            SetupContentPanel();
            SetupConnectionChoicePanel();
            SetupJoinGamePanel();
            SetupHostGamePanel();
        }

        private void InstantiatePanels() {
            _connectionChoicePanelInstantiated = _connectionChoicePanel.Instantiate();
            _connectionChoicePanelInstantiated.style.flexGrow = 1;

            _joinGamePanelInstantiated = _joinGamePanel.Instantiate();
            _joinGamePanelInstantiated.style.flexGrow = 1;

            _hostGamePanelInstantiated = _hostGamePanel.Instantiate();
            _hostGamePanelInstantiated.style.flexGrow = 1;
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

            _joinGamePanelInstantiated
                .Q<Button>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.JoinButton)
                .RegisterCallback<ClickEvent>(OnClickJoinAsClient);
            _joinGamePanelInstantiated
                .Q<Button>(Constants.VisualElementNames.ConnectionMenu.JoinGamePanel.CancelButton)
                .RegisterCallback<ClickEvent>(OnClickCancel);
        }

        private void SetupHostGamePanel() {
            _hostIPInput = _hostGamePanelInstantiated.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.JoinIP);
            _hostPortInput = _hostGamePanelInstantiated.Q<TextField>(Constants.VisualElementNames.ConnectionMenu.HostGamePanel.JoinPort);

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

        private void OnClickJoinAsClient(ClickEvent evt) {
            Debug.Log("Join");
        }

        private void OnClickHostButton(ClickEvent evt) {
            Debug.Log("Host");
        }

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
