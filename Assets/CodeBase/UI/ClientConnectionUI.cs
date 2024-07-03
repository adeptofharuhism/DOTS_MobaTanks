using Assets.CodeBase.Network;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        private TextField _joinPlayerName;
        private TextField _joinIPInput;
        private TextField _joinPortInput;

        private VisualElement _hostGamePanelInstantiated;
        private TextField _hostPlayerName;
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

        private void OnClickJoinAsClient(ClickEvent evt) {
            DestroyLocalSimulationWorld();
            SceneManager.LoadScene(1);

            StartClient(_joinIPInput.text, ushort.Parse(_joinPortInput.text), _joinPlayerName.text);
        }

        private void OnClickHostButton(ClickEvent evt) {
            DestroyLocalSimulationWorld();
            SceneManager.LoadScene(1);

            StartServer(ushort.Parse(_hostPortInput.text));
            StartClient(LocalHostIP, ushort.Parse(_hostPortInput.text), _hostPlayerName.text);
        }

        private void OnClickCancel(ClickEvent evt) {
            ClearContentPanel();
            _contentPanel.Add(_connectionChoicePanelInstantiated);
        }

        private void ClearContentPanel() {
            if (_contentPanel.childCount > 0)
                _contentPanel.RemoveAt(0);
        }

        #region Rewrite As Service
        private const string LocalHostIP = "127.0.0.1";

        private void DestroyLocalSimulationWorld() {
            foreach (World world in World.All)
                if (world.Flags == WorldFlags.Game) {
                    world.Dispose();
                    break;
                }
        }

        private void StartServer(ushort port) {
            World serverWorld = ClientServerBootstrap.CreateServerWorld(Constants.WorldNames.ServerWorld);

            NetworkEndpoint serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(port);

            using (EntityQuery networkDriverQuery =
                serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>()))
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
        }

        private void StartClient(string ipAddress, ushort port, string playerName) {
            World clientWorld = ClientServerBootstrap.CreateClientWorld(Constants.WorldNames.ClientWorld);
            World.DefaultGameObjectInjectionWorld = clientWorld;

            NetworkEndpoint connectionEndpoint = NetworkEndpoint.Parse(ipAddress, port);

            using (EntityQuery networkDriverQuery =
                clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>()))
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, connectionEndpoint);

            Entity connectionDataEntity = clientWorld.EntityManager.CreateEntity();
            clientWorld.EntityManager.AddComponentData(connectionDataEntity, new ConnectionRequestData {
                PlayerName = playerName
            });
        }
        #endregion
    }
}
