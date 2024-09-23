using Assets.CodeBase.Infrastructure.Services.SceneLoader;
using Assets.CodeBase.Infrastructure.Services.WorldControl;
using Assets.CodeBase.Infrastructure.StateMachine.States;
using System;
using System.Collections.Generic;
using Zenject;

namespace Assets.CodeBase.Infrastructure.StateMachine
{
    public class GameStateMachine : IInitializable, IGameStateMachine 
    {
        private readonly Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;

        [Inject]
        public GameStateMachine(ISceneLoader sceneLoader, IWorldControlService worldControlService) {
            _states = new Dictionary<Type, IExitableState> {
                [typeof(BootstrapState)] = new BootstrapState(this),
                [typeof(LoadStartSceneState)] = new LoadStartSceneState(this, sceneLoader),
                [typeof(StartSceneActiveState)] = new StartSceneActiveState(this),
                [typeof(LoadMainSceneState)] = new LoadMainSceneState(this, sceneLoader, worldControlService),
                [typeof(MainSceneActiveState)] = new MainSceneActiveState(this, worldControlService)
            };
        }

        public void Initialize() => 
            Enter<BootstrapState>();

        public void Enter<TState>() where TState : class, IState {
            IState state = ChangeState<TState>();

            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload> {
            IPayloadedState<TPayload> state = ChangeState<TState>();

            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState {
            _activeState?.Exit();

            TState state = GetState<TState>();
            _activeState = state;
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"Entering {typeof(TState).Name}");
#endif
            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState =>
            _states[typeof(TState)] as TState;
    }
}
