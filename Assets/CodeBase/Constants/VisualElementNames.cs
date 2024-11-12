namespace Assets.CodeBase.Constants
{
	public static class VisualElementNames
	{
		public const string ContentPanel = "ContentPanel";

		public static class ConnectionMenu
		{
			public static class ConnectionChoicePanel
			{
				public const string HostButton = "HostButton";
				public const string JoinButton = "ClientButton";
				public const string ExitButton = "ExitButton";
			}

			public abstract class ConnectionGamePanel
			{
				public const string PlayerName = "PlayerName";
				public const string JoinPort = "JoinPort";
				public const string CancelButton = "CancelButton";
			}

			public abstract class JoinGamePanel : ConnectionGamePanel
			{
				public const string JoinIP = "JoinIP";
				public const string JoinButton = "JoinButton";
			}

			public abstract class HostGamePanel : ConnectionGamePanel
			{
				public const string HostButton = "HostButton";
			}
		}

		public static class GameUI
		{
			public static class PreparingPanel
			{
				public const string SubContentPanel = "PreparingPanel";

				public static class AskReadyPanel
				{
					public const string ReadyButton = "ReadyButton";
				}
			}

			public static class InGamePanel
			{
				public const string MoneyLabel = "MoneyLabel";
				public const string ShopButton = "ShopButton";

				public const string ItemGroupContainer = "ItemGroupContainer";

				public const string Inventory = "Inventory";

				public static class ItemGroup
				{
					public const string ItemGroupName = "ItemGroupName";
					public const string ItemContainer = "ItemContainer";
				}

				public static class ShopButtonTemplate
				{
					public const string Button = "Button";
					public const string Label = "Label";
				}
			}

			public static class EndGamePanel
			{
				public const string WinnerAssetContainer = "WinnerAssetContainer";
				public const string DisconnectButton = "DiconnectButton";
			}
		}
	}
}