using Assets.CodeBase.UI.StartScene.Panels;
using Assets.CodeBase.Utility.MVVM;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Assets.CodeBase.UI.StartScene
{
    public class StartSceneView : View, IInitializable, IDisposable
    {
        [SerializeField] private VisualTreeAsset _connectionChoicePanel;
        [SerializeField] private VisualTreeAsset _joinGamePanel;
        [SerializeField] private VisualTreeAsset _hostGamePanel;

        private IStartSceneViewModel _startSceneViewModel;

        [Inject]
        public void Construct(IStartSceneViewModel startSceneViewModel) {
            _startSceneViewModel = startSceneViewModel;

            AddPanel(new ConnectionChoicePanel(_connectionChoicePanel, startSceneViewModel));
            AddPanel(new JoinPanel(_joinGamePanel, startSceneViewModel));
            AddPanel(new HostPanel(_hostGamePanel, startSceneViewModel));
        }

        public override void Initialize() {
            base.Initialize();

            ActivatePanel<ConnectionChoicePanel>();
        }

        protected override void SubscribeToViewModel() =>
            _startSceneViewModel.Mode.OnChanged += OnChangeMode;

        protected override void UnsubscribeFromViewModel() =>
            _startSceneViewModel.Mode.OnChanged -= OnChangeMode;

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
