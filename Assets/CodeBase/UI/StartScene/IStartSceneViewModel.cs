using Assets.CodeBase.Utility;

namespace Assets.CodeBase.UI.StartScene
{
    public interface IConnectionVariantViewModel
    {
        ReactiveProperty<string> PlayerNameView { get; }

        void OnClickCancel();
        void OnFocusOutPlayerName(string name);
    }

    public interface IJoinVariantViewModel : IConnectionVariantViewModel
    {
        ReactiveProperty<string> JoinPortView { get; }
        ReactiveProperty<string> JoinIpView { get; }

        void OnClickJoinGame();
        void OnFocusOutIp(string ip);
        void OnFocusOutJoinPort(string port);
    }

    public interface IHostVariantViewModel : IConnectionVariantViewModel
    {
        ReactiveProperty<string> HostPortView { get; }

        void OnClickHostGame();
        void OnFocusOutHostPort(string port);
    }

    public interface IConnectionChoiceViewModel
    {
        void OnClickExit();
        void OnClickHostConnectionVariant();
        void OnClickJoinConnectionVariant();
    }

    public interface IStartSceneViewModel : IConnectionChoiceViewModel, IJoinVariantViewModel, IHostVariantViewModel
    {
        ReactiveProperty<StartSceneMode> Mode { get; }
    }
}
