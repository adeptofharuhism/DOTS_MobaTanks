using Assets.CodeBase.Infrastructure.Services.Inventory;
using Assets.CodeBase.Infrastructure.Services.ItemDescriptionAccess;
using Assets.CodeBase.Infrastructure.Services.UiFactories;
using Assets.CodeBase.Inventory.Items;
using Assets.CodeBase.UI.MainScene;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Assets.CodeBase.Infrastructure.Installers
{
	public class MainSceneUiInstaller : MonoInstaller
	{
		[SerializeField] private MainSceneView _mainSceneView;
		[SerializeField] private ItemCollection _itemCollection;
		[SerializeField] private VisualTreeAsset _itemGroup;
		[SerializeField] private VisualTreeAsset _shopButtonTemplate;

		public override void InstallBindings() {
			RegisterItemDescriptionAccessService();
			RegisterItemButtonFactory();
			RegisterInventoryService();

			RegisterMainSceneViewModel();
			RegisterMainSceneView();
		}

		private void RegisterItemDescriptionAccessService() =>
			Container
				.Bind<IItemDescriptionAccess>()
				.To<ItemDescriptionAccessService>()
				.FromNew()
				.AsSingle()
				.WithArguments(_itemCollection)
				.NonLazy();

		private void RegisterItemButtonFactory() =>
			Container
				.BindInterfacesTo<ItemButtonFactory>()
				.FromNew()
				.AsSingle()
				.WithArguments(_itemGroup, _shopButtonTemplate)
				.NonLazy();

		private void RegisterInventoryService() =>
			Container
				.BindInterfacesTo<InventoryService>()
				.FromNew()
				.AsSingle()
				.NonLazy();

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