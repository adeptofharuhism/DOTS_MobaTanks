using Assets.CodeBase.Infrastructure.StateMachine.States;

namespace Assets.CodeBase.Infrastructure.StateMachine
{
    public interface IGameStateMachine
    {
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
        void Enter<TState>() where TState : class, IState;
    }
}