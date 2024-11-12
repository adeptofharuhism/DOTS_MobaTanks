using Assets.CodeBase.Infrastructure.Services.UiFactories;
using Assets.CodeBase.Utility.MVVM;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.MainScene.Panels
{
	public class InGamePanel : UiPanel
	{
		private readonly List<UiPart> _uiParts = new();

		public InGamePanel(
			VisualTreeAsset inGamePanel,
			IInGameModeViewModel inGameModeViewModel,
			IInventoryButtonFactory inventoryButtonFactory,
			IShopButtonFactory shopButtonFactory)
			: base(inGamePanel) {

			_uiParts.Add(new ShopPart(_panel, inGameModeViewModel, shopButtonFactory));
			_uiParts.Add(new MoneyDisplayPart(_panel, inGameModeViewModel));
			_uiParts.Add(new InventoryPart(_panel, inGameModeViewModel, inventoryButtonFactory));
		}

		protected override void InitializeSubParts() {
			foreach (UiPart uiPart in _uiParts)
				uiPart.Initialize();
		}

		protected override void DisposeSubParts() {
			foreach (UiPart uiPart in _uiParts)
				uiPart.Dispose();
		}
	}
}