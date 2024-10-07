using Assets.CodeBase.Infrastructure.Services.MainSceneModeNotifier;
using Assets.CodeBase.UI.MainScene.Panels;
using Assets.CodeBase.Utility.MVVM;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Assets.CodeBase.UI.MainScene
{
    public class MainSceneView : View
    {
        [Header("Preparing mode")]
        [SerializeField] private VisualTreeAsset _preparingPanel;
        [SerializeField] private VisualTreeAsset _askReadyPanel;
        [SerializeField] private VisualTreeAsset _waitingPanel;
        [Header("In Game mode")]
        [SerializeField] private VisualTreeAsset _inGamePanel;
        [Header("End Game mode")]
        [SerializeField] private VisualTreeAsset _endGamePanel;

        private IMainSceneViewModel _mainSceneViewModel;

        [Inject]
        public void Construct(IMainSceneViewModel mainSceneViewModel) {
            _mainSceneViewModel = mainSceneViewModel;

            AddPanel(new PreparingPanel(_preparingPanel, mainSceneViewModel, _askReadyPanel, _waitingPanel));
        }

        public override void Initialize() {
            base.Initialize();
        }

        protected override void SubscribeToViewModel() {
            _mainSceneViewModel.Mode.OnChanged += OnChangeMode;
        }

        protected override void UnsubscribeFromViewModel() {
            _mainSceneViewModel.Mode.OnChanged -= OnChangeMode;
        }

        private void OnChangeMode(MainSceneMode mode) {
            switch (mode) {
                default:
                case MainSceneMode.Loading:
                    DeactivateActivePanel();
                    break;
                case MainSceneMode.Preparing:
                    ActivatePanel<PreparingPanel>();
                    break;
                case MainSceneMode.InGame:
                    break;
                case MainSceneMode.GameOver:
                    break;
            }
        }
    }
}
