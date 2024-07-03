using Assets.CodeBase.Network;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private VisualTreeAsset _gameReadyPanel;

        private VisualElement _buttonsPart;

        private VisualElement _gameReadyPanelInstantiated;

        private void OnEnable() {
            InstantiatePanels();
            SetupButtonsPart();
            SetupGameReadyPanel();
        }

        private void InstantiatePanels() {
            _gameReadyPanelInstantiated = _gameReadyPanel.Instantiate();
            _gameReadyPanelInstantiated.style.flexGrow = 1;
        }

        private void SetupButtonsPart() {
            _buttonsPart = _uiDocument.rootVisualElement.Q<VisualElement>(Constants.VisualElementNames.GameUI.ButtonsPart);

            _buttonsPart.Add(_gameReadyPanelInstantiated);
        }

        private void SetupGameReadyPanel() {
            _gameReadyPanelInstantiated
                .Q<Button>(Constants.VisualElementNames.GameUI.GameReadyPanel.ReadyButton)
                .RegisterCallback<ClickEvent>(OnClickReadyButton);
        }

        private void OnClickReadyButton(ClickEvent evt) {
            ClearButtonsPartPanel();
            SendReadyRpc();
        }

        private void ClearButtonsPartPanel() {
            if (_buttonsPart.childCount > 0)
                _buttonsPart.RemoveAt(0);
        }

        private void SendReadyRpc() =>
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(ReadyRpc), typeof(SendRpcCommandRequest));
    }
}
