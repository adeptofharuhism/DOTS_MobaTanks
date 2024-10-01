using Assets.CodeBase.UI.StartScene;
using UnityEngine;
using Zenject;

namespace Assets.CodeBase.Infrastructure.Installers
{
    public class StartSceneUiInstaller : MonoInstaller
    {
        [SerializeField] private StartSceneView _startSceneView;

        public override void InstallBindings() {
            RegisterViewModel();
            RegisterView();
        }

        private void RegisterViewModel() => 
            Container
                .BindInterfacesTo<StartSceneViewModel>()
                .FromNew()
                .AsSingle()
                .NonLazy();

        private void RegisterView() => 
            Container
                .BindInterfacesTo<StartSceneView>()
                .FromInstance(_startSceneView)
                .AsSingle()
                .NonLazy();
    }
}
