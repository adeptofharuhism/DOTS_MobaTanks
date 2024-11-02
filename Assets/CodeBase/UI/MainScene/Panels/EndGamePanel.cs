using Assets.CodeBase.Teams;
using Assets.CodeBase.Utility.MVVM;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.MainScene.Panels
{
	public class EndGamePanel : UiPanel
	{
		private readonly VisualElement _blueWinnerText;
		private readonly VisualElement _orangeWinnerText;

		private Button _disconnectButton;
		private VisualElement _winnerAssetContainer;

		private readonly IEndGameModeViewModel _endGameModeViewModel;

		public EndGamePanel(
			VisualTreeAsset panelAsset,
			IEndGameModeViewModel endGameModeViewModel,
			VisualTreeAsset blueWinnerAsset,
			VisualTreeAsset orangeWinnerAsset)
			: base(panelAsset) {

			_endGameModeViewModel = endGameModeViewModel;

			_blueWinnerText = blueWinnerAsset.InstantiatePanel();
			_orangeWinnerText = orangeWinnerAsset.InstantiatePanel();
		}

		protected override void CacheVisualElements() {
			_disconnectButton = 
				_panel.Q<Button>(Constants.VisualElementNames.GameUI.EndGamePanel.DisconnectButton);
			_winnerAssetContainer =
				_panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.EndGamePanel.WinnerAssetContainer);
		}

		protected override void RegisterCallbacks() {
			_disconnectButton.RegisterCallback<ClickEvent>(OnClickDisconnect);
		}

		protected override void UnregisterCallbacks() {
			_disconnectButton.UnregisterCallback<ClickEvent>(OnClickDisconnect);
		}

		protected override void BindData() {
			_endGameModeViewModel.OnEndGame += ShowWinnerText;
		}

		protected override void UnbindData() {
			_endGameModeViewModel.OnEndGame -= ShowWinnerText;
		}

		private void OnClickDisconnect(ClickEvent evt) {
			_endGameModeViewModel.Disconnect();
		}

		private void ShowWinnerText(TeamType type) {
			switch (type) {
				default:
				case TeamType.None:
				case TeamType.Blue:
					AddWinnerAsset(_blueWinnerText);

					break;
				case TeamType.Orange:
					AddWinnerAsset(_orangeWinnerText);

					break;
			}
		}

		private void AddWinnerAsset(VisualElement winnerText) {
			_winnerAssetContainer.Add(winnerText);
		}
	}
}