using Assets.CodeBase.Combat.Teams;
using Assets.CodeBase.GameStates.GameStart;
using Assets.CodeBase.Infrastructure.PlayerCount;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private VisualTreeAsset _loadingPanel;
        [SerializeField] private VisualTreeAsset _gameReadyPanel;
        [SerializeField] private VisualTreeAsset _blueWonPanel;
        [SerializeField] private VisualTreeAsset _orangeWonPanel;

        private VisualElement _buttonsPart;

        private VisualElement _loadingPanelInstantiated;
        private VisualElement _gameReadyPanelInstantiated;
        private VisualElement _blueWonPanelInstantiated;
        private VisualElement _orangeWonPanelInstantiated;

        private void OnEnable() {
            InstantiatePanels();
            SetupButtonsPart();
            SetupGameReadyPanel();
            SetupEndGamePanel(_blueWonPanelInstantiated);
            SetupEndGamePanel(_orangeWonPanelInstantiated);

            ConnectSystems();
        }

        private void OnDisable() {
            DisconnectSystems();
        }

        private void InstantiatePanels() {
            _loadingPanelInstantiated = InstantiatePanel(_loadingPanel);
            _gameReadyPanelInstantiated = InstantiatePanel(_gameReadyPanel);
            _blueWonPanelInstantiated = InstantiatePanel(_blueWonPanel);
            _orangeWonPanelInstantiated = InstantiatePanel(_orangeWonPanel);
        }

        private void SetupButtonsPart() {
            _buttonsPart = _uiDocument.rootVisualElement.Q<VisualElement>(Constants.VisualElementNames.GameUI.ButtonsPart);

            _buttonsPart.Add(_loadingPanelInstantiated);
        }

        private void SetupGameReadyPanel() {
            _gameReadyPanelInstantiated
                .Q<Button>(Constants.VisualElementNames.GameUI.GameReadyPanel.ReadyButton)
                .RegisterCallback<ClickEvent>(OnClickReadyButton);
        }

        private void SetupEndGamePanel(VisualElement panel) {
            panel
                .Q<Button>(Constants.VisualElementNames.GameUI.EndGamePanel.EndGameButton)
                .RegisterCallback<ClickEvent>(OnClickEndGameButton);
        }

        private void ConnectSystems() {
            if (World.DefaultGameObjectInjectionWorld == null)
                return;

            DeployUiOnClientSystem deployUiSystem =
                World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<DeployUiOnClientSystem>();
            if (deployUiSystem != null)
                deployUiSystem.OnReadyForUiDeploy += ShowGameReadyPanel;

            ClientEnterEndGameSystem endGameSystem =
                World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ClientEnterEndGameSystem>();
            if (endGameSystem != null)
                endGameSystem.OnEndGame += ShowEndGamePanel;
        }

        private void DisconnectSystems() {
            if (World.DefaultGameObjectInjectionWorld == null)
                return;

            DeployUiOnClientSystem deployUiSystem =
                World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<DeployUiOnClientSystem>();
            if (deployUiSystem != null)
                deployUiSystem.OnReadyForUiDeploy -= ShowGameReadyPanel;

            ClientEnterEndGameSystem endGameSystem =
                World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ClientEnterEndGameSystem>();
            if (endGameSystem != null)
                endGameSystem.OnEndGame -= ShowEndGamePanel;
        }

        private VisualElement InstantiatePanel(VisualTreeAsset panel) {
            VisualElement instantiatedPanel = panel.Instantiate();
            instantiatedPanel.style.flexGrow = 1;

            return instantiatedPanel;
        }

        private void OnClickReadyButton(ClickEvent evt) {
            ClearButtonsPartPanel();
            SendReadyRpc();
        }

        private void OnClickEndGameButton(ClickEvent evt) {
            using (EntityQuery networkStreamQuery =
                World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamConnection))) {

                if (networkStreamQuery.TryGetSingletonEntity<NetworkStreamConnection>(out var networkStream))
                    World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<NetworkStreamRequestDisconnect>(networkStream);
            }

            World.DisposeAllWorlds();

            SceneManager.LoadScene(0);
        }

        private void ClearButtonsPartPanel() {
            if (_buttonsPart.childCount > 0)
                _buttonsPart.RemoveAt(0);
        }

        private void SendReadyRpc() =>
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(ReadyRpc), typeof(SendRpcCommandRequest));

        private void ShowGameReadyPanel() {
            ClearButtonsPartPanel();

            _buttonsPart.Add(_gameReadyPanelInstantiated);
        }

        private void ShowEndGamePanel(TeamType team) {
            ClearButtonsPartPanel();

            if (team == TeamType.Blue)
                _buttonsPart.Add(_blueWonPanelInstantiated);
            else
                _buttonsPart.Add(_orangeWonPanelInstantiated);
        }
    }
}
