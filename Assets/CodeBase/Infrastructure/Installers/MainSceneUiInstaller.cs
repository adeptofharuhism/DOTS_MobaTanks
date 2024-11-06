using Assets.CodeBase.Infrastructure.Services.Inventory;
using Assets.CodeBase.Inventory.Items;
using Assets.CodeBase.UI.MainScene;
using UnityEngine;
using Zenject;

namespace Assets.CodeBase.Infrastructure.Installers
{
    public class MainSceneUiInstaller : MonoInstaller
    {
        [SerializeField] private MainSceneView _mainSceneView;
        [SerializeField] private ItemCollection _itemCollection;

        public override void InstallBindings() {
            RegisterInventoryService();
            
            RegisterMainSceneViewModel();
            RegisterMainSceneView();
        }

        private void RegisterInventoryService() {
            Container
                .BindInterfacesTo<InventoryService>()
                .FromNew()
                .AsSingle()
                .WithArguments(_itemCollection)
                .NonLazy();
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
