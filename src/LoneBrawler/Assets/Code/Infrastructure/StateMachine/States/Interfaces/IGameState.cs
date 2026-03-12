// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.StateMachine.Types;

namespace Code.Infrastructure.StateMachine.States.Interfaces
{
  public interface IGameState : IGameExitableState
  {
    public void Enter();
  }
  public interface IGamePayloadedState<TPayload> : IGameExitableState
  {
    public void Enter(TPayload payload);
  }

  public interface IGameExitableState
  {
    public StateType Type { get; }

    public void Exit();
  }
}
