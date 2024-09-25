namespace Assets.CodeBase.Constants
{
    public static class VisualElementNames
    {
        public static class ConnectionMenu
        {
            public const string ContentPanel = "ContentPanel";

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
            public const string ButtonsPart = "ButtonsPart";
            public const string MoneyPart = "MoneyPart";
            public const string ShopPart = "ShopPart";

            public class GameReadyPanel
            {
                public const string ReadyButton = "ReadyButton";
            }

            public class EndGamePanel
            {
                public const string EndGameButton = "EndGameButton";
            }

            public class MoneyPanel
            {
                public const string MoneyAmount = "MoneyAmount";
                public const string ShopButton = "ShopButton";
            }
        }
    }
}
