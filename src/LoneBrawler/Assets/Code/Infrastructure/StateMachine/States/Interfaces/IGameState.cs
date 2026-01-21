// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

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
    public void Exit();
  }

  public interface IStateDepsReader : IGameExitableState
  {
    void ReadDependencies(GameStateDependencies gameStateDependencies);
  }
}
