// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States.Interfaces;

using Code.Infrastructure.StateMachine.Types;
using Code.Infrastructure.StateMachine.Factory;

namespace Code.Infrastructure.StateMachine
{
  public class GameStateMachine : IGameStateMachine
  {
    private IGameExitableState _activeState;

    private readonly StateFactory _stateFactory;

    public GameStateMachine(StateFactory stateFactory) => _stateFactory = stateFactory;

    public StateType GetCurrentState() => _activeState.Type;

    public void EnterState<TState>()
      where TState : class, IGameState
    {
      IGameState gameState = ChangeState<TState>(); // downcast
      gameState.Enter();
    }

    public void EnterState<TState, TPayload>(TPayload payload)
      where TState : class, IGamePayloadedState<TPayload>
    {
      IGamePayloadedState<TPayload> gameState = ChangeState<TState>(); // downcast
      gameState.Enter(payload);
    }

    private TState ChangeState<TState>() where TState : class, IGameExitableState
    {
      _activeState?.Exit();
      TState gameState = _stateFactory.CreateState<TState>();
      _activeState = gameState;
      return gameState;
    }
  }
}
