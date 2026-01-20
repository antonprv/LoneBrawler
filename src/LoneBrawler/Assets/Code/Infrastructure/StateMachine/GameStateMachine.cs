// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using Code.Common.Extensions.Async;
using Code.Gameplay.Common.Visuals.UI;
using Code.Infrastructure.StateMachine.States;

namespace Code.Infrastructure.StateMachine
{
  public class GameStateMachine
  {
    private Dictionary<Type, IGameExitableState> _states;
    private IGameExitableState _activeState;

    public GameStateMachine(ICoroutineRunner runner, ILoadScreen curtain)
    {
      _states = new Dictionary<Type, IGameExitableState>()
      {
        [typeof(BootStrapperState)] = new BootStrapperState(this, runner),
        [typeof(LoadLevelState)] = new LoadLevelState(this, runner, curtain),
        [typeof(GameLoopState)] = new GameLoopState(this, runner)
      };
    }

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
      TState gameState = GetGameState<TState>(); // implicit cast
      _activeState = gameState;
      return gameState;
    }

    private TState GetGameState<TState>() where TState : class, IGameExitableState =>
      _states[typeof(TState)] as TState;
  }
}
