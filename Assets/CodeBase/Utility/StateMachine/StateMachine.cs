using System;
using System.Collections.Generic;

namespace Assets.CodeBase.Utility.StateMachine
{
    public abstract class StateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;

        protected StateMachine() =>
            _states = new Dictionary<Type, IExitableState>();

        protected void AddState<TState>(TState state) where TState : class, IExitableState, IStateRestriction =>
            _states.Add(typeof(TState), state);

        protected void Enter<TState>() where TState : class, IState {
            IState state = ChangeState<TState>();

            state.Enter();
        }

        protected void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload> {
            IPayloadedState<TPayload> state = ChangeState<TState>();

            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState {
            _activeState?.Exit();

            TState state = GetState<TState>();
            _activeState = state;
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"Entering {typeof(TState).Name} in {GetType().Name}");
#endif
            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState =>
            _states[typeof(TState)] as TState;
    }
}
