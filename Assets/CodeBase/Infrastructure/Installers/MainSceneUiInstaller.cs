using Assets.CodeBase.UI.MainScene;
using UnityEngine;
using Zenject;

namespace Assets.CodeBase.Infrastructure.Installers
{
    public class MainSceneUiInstaller : MonoInstaller
    {
        [SerializeField] private MainSceneView _mainSceneView;

        public override void InstallBindings() {
            RegisterMainSceneViewModel();
            RegisterMainSceneView();
        }

        private void RegisterMainSceneViewModel() => 
            Container
                .BindInterfacesTo<MainSceneViewModel>()
                .FromNew()
                .AsSingle()
                .NonLazy();

        private void RegisterMainSceneView() => 
            Container
                .BindInterfacesTo<MainSceneView>()
                .FromInstance(_mainSceneView)
                .AsSingle()
                .NonLazy();
    }
}
