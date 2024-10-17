using Assets.CodeBase.Teams;
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
        [Header("End game")]
        [SerializeField] private VisualTreeAsset _blueWonPanel;
        [SerializeField] private VisualTreeAsset _orangeWonPanel;
        [Header("Shop")]
        [SerializeField] private VisualTreeAsset _moneyPanel;
        [SerializeField] private VisualTreeAsset _shopPanel;

        private VisualElement _buttonsPart;
        private VisualElement _moneyPart;
        private VisualElement _shopPart;

        private VisualElement _blueWonPanelInstantiated;
        private VisualElement _orangeWonPanelInstantiated;

        private bool _shopIsAvailable;
        private VisualElement _moneyPanelInstantiated;
        private Label _moneyAmount;
        private VisualElement _shopPanelInstantiated;

        private void OnEnable() {
            InstantiatePanels();
            SetupParts();
            SetupEndGamePanel(_blueWonPanelInstantiated);
            SetupEndGamePanel(_orangeWonPanelInstantiated);
            SetupMoneyPanel();
        }

        private void InstantiatePanels() {
            _blueWonPanelInstantiated = InstantiatePanel(_blueWonPanel);
            _orangeWonPanelInstantiated = InstantiatePanel(_orangeWonPanel);
            _moneyPanelInstantiated = InstantiatePanel(_moneyPanel);
            _shopPanelInstantiated = InstantiatePanel(_shopPanel);
        }

        private void SetupParts() {
            _buttonsPart = _uiDocument.rootVisualElement
                .Q<VisualElement>("Constants.VisualElementNames.GameUI.ButtonsPart");

            _moneyPart = _uiDocument.rootVisualElement
                .Q<VisualElement>("Constants.VisualElementNames.GameUI.MoneyPart");
            _shopPart = _uiDocument.rootVisualElement
                .Q<VisualElement>("Constants.VisualElementNames.GameUI.ShopPart");
        }

        private void SetupEndGamePanel(VisualElement panel) {
            panel
                .Q<Button>("Constants.VisualElementNames.GameUI.EndGamePanel.EndGameButton")
                .RegisterCallback<ClickEvent>(OnClickEndGameButton);
        }

        private void SetupMoneyPanel() {
            _moneyAmount = _moneyPanelInstantiated
                .Q<Label>("Constants.VisualElementNames.GameUI.MoneyPanel.MoneyAmount");

            _moneyPanelInstantiated
                .Q<Button>("Constants.VisualElementNames.GameUI.MoneyPanel.ShopButton")
                .RegisterCallback<ClickEvent>(OnClickShopButton);
        }

        private VisualElement InstantiatePanel(VisualTreeAsset panel) {
            VisualElement instantiatedPanel = panel.Instantiate();
            instantiatedPanel.style.flexGrow = 1;

            return instantiatedPanel;
        }

        private void OnClickEndGameButton(ClickEvent evt) {
            using (EntityQuery networkStreamQuery =
                World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamConnection))) {

                if (networkStreamQuery.TryGetSingletonEntity<NetworkStreamConnection>(out var networkStream))
                    World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<NetworkStreamRequestDisconnect>(networkStream);
            }

            World.DisposeAllWorlds();

            SceneManager.LoadScene(Constants.SceneNames.StartSceneName);
        }

        private void OnClickShopButton(ClickEvent evt) {
            if (_shopPart.childCount > 0)
                _shopPart.RemoveAt(0);
            else
                if (_shopIsAvailable)
                _shopPart.Add(_shopPanelInstantiated);
        }

        private void ClearButtonsPartPanel() {
            if (_buttonsPart.childCount > 0)
                _buttonsPart.RemoveAt(0);
        }

        private void ShowEndGamePanel(TeamType team) {
            ClearButtonsPartPanel();

            if (team == TeamType.Blue)
                _buttonsPart.Add(_blueWonPanelInstantiated);
            else
                _buttonsPart.Add(_orangeWonPanelInstantiated);
        }

        private void ShowInGameUi() {
            ClearButtonsPartPanel();

            _moneyPart.Add(_moneyPanelInstantiated);
        }

        private void UpdateMoneyAmount(int money) {
            _moneyAmount.text = money.ToString();
        }

        private void SetShopAvailability(bool shopAvailability) {
            if (!shopAvailability)
                ClearShopPart();

            _shopIsAvailable = shopAvailability;
        }

        private void ClearShopPart() {
            if (_shopPart.childCount > 0)
                _shopPart.RemoveAt(0);
        }
    }
}
