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
                public const string LeftPart = "LeftPart";
                public const string CentralPart = "CentralPart";
                public const string RightPart = "RightPart";

                public static class ShopPanel
                {
                    public const string ShopPart = "ShopPart";
                    public const string ShopButton = "ShopButton";
                    public const string MoneyLabel = "MoneyLabel";
                }
            }

            public static class EndGamePanel { }
        }
    }
}
