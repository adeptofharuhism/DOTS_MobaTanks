using Assets.CodeBase.Infrastructure.GameStateManagement.States;
using Assets.CodeBase.Infrastructure.Services.SceneLoader;
using Assets.CodeBase.Infrastructure.Services.WorldControl;
using Assets.CodeBase.UI.Curtain;
using Assets.CodeBase.Utility.StateMachine;
using Zenject;

namespace Assets.CodeBase.Infrastructure.GameStateManagement
{
    public interface IGameState : IStateRestriction { }

    public interface IGameStateMachine
    {
        void EnterGameState<TState>()
            where TState : class, IState, IGameState;

        void EnterGameState<TState, TPayload>(TPayload payload)
            where TState : class, IPayloadedState<TPayload>, IGameState;
    }

    public class GameStateMachine : StateMachine, IInitializable, IGameStateMachine
    {
        [Inject]
        public GameStateMachine(ISceneLoader sceneLoader, IWorldControlService worldControlService, ILoadingCurtain loadingCurtain) {
            AddGameState(new BootstrapState(this));
            AddGameState(new LoadStartSceneState(this, sceneLoader, worldControlService, loadingCurtain));
            AddGameState(new StartSceneActiveState(this));
            AddGameState(new LoadMainSceneState(this, sceneLoader, worldControlService, loadingCurtain));
            AddGameState(new PrepareForGameState(this));
        }

        public void Initialize() =>
            EnterGameState<BootstrapState>();

        public void EnterGameState<TState>()
            where TState : class, IState, IGameState =>
            Enter<TState>();

        public void EnterGameState<TState, TPayload>(TPayload payload)
            where TState : class, IPayloadedState<TPayload>, IGameState =>
            Enter<TState, TPayload>(payload);

        private void AddGameState<TState>(TState state) 
            where TState : class, IExitableState, IGameState =>
            AddState(state);
    }
}
