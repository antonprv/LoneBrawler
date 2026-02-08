// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.StateMachine.States.Interfaces;

namespace Code.Infrastructure.StateMachine.Interfaces
{
  public interface IGameStateMachine
  {
    public void EnterState<TState, TPayload>(TPayload payload) where TState : class, IGamePayloadedState<TPayload>;
    public void EnterState<TState>() where TState : class, IGameState;
  }
}
