using Assets.CodeBase.Utility.MVVM;
using UnityEngine.UIElements;

namespace Assets.CodeBase.UI.MainScene.Panels
{
	public class PreparingPanel : UiPanel
	{
		private VisualElement _subContentPanel;

		private readonly AskReadyPanel _askReadyPanel;
		private readonly WaitingPanel _waitingPanel;

		private readonly INotifyReadyViewModel _notifyReadyViewModel;

		public PreparingPanel(
			VisualTreeAsset preparingPanel,
			IPreparingModeViewModel preparingModeViewModel,
			VisualTreeAsset askReadyPanel,
			VisualTreeAsset waitingPanel)
			: base(preparingPanel) {

			_notifyReadyViewModel = preparingModeViewModel;

			_askReadyPanel = new AskReadyPanel(askReadyPanel, preparingModeViewModel);
			_waitingPanel = new WaitingPanel(waitingPanel);
		}

		protected override void CacheVisualElements() {
			_subContentPanel =
				_panel.Q<VisualElement>(Constants.VisualElementNames.GameUI.PreparingPanel.SubContentPanel);
		}

		protected override void InitializeSubParts() {
			_askReadyPanel.Initialize();
			
			AddPanelToSubContent(_askReadyPanel);
		}
		
		protected override void DisposeSubParts() {
			RemovePanelFromSubContent(_waitingPanel);
			
			_askReadyPanel.Dispose();
		}

		protected override void BindData() {
			_notifyReadyViewModel.OnReady += OnReady;
		}

		protected override void UnbindData() {
			_notifyReadyViewModel.OnReady -= OnReady;
		}

		private void AddPanelToSubContent(UiPanel panel) =>
			_subContentPanel.Add(panel.Panel);

		private void RemovePanelFromSubContent(UiPanel panel) =>
			_subContentPanel.Remove(panel.Panel);

		private void OnReady() =>
			SwitchPanels();

		private void SwitchPanels() {
			RemovePanelFromSubContent(_askReadyPanel);
			AddPanelToSubContent(_waitingPanel);
		}
	}

	public class AskReadyPanel : UiPanel
	{
		private Button _readyButton;

		private readonly IAskReadyViewModel _askReadyViewModel;

		public AskReadyPanel(VisualTreeAsset panelAsset, IAskReadyViewModel askReadyViewModel)
			: base(panelAsset) {

			_askReadyViewModel = askReadyViewModel;
		}

		protected override void CacheVisualElements() {
			_readyButton =
				_panel.Q<Button>(Constants.VisualElementNames.GameUI.PreparingPanel.AskReadyPanel.ReadyButton);
		}

		protected override void RegisterCallbacks() {
			_readyButton.RegisterCallback<ClickEvent>(OnClickReady);
		}

		protected override void UnregisterCallbacks() {
			_readyButton.UnregisterCallback<ClickEvent>(OnClickReady);
		}

		private void OnClickReady(ClickEvent evt) => 
			_askReadyViewModel.SendReadyRpc();
	}

	public class WaitingPanel : UiPanel
	{
		public WaitingPanel(VisualTreeAsset panelAsset)
			: base(panelAsset) { }
	}
}