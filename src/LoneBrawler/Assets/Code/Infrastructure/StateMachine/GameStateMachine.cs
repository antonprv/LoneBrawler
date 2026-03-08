// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;
using Code.Infrastructure.StateMachine.States.Interfaces;

using Zenjex.Extensions.Lifecycle;

namespace Code.Infrastructure.StateMachine
{
  public class GameStateMachine : IInitializable, IGameStateMachine
  {
    private Dictionary<Type, IGameExitableState> _states;
    private IGameExitableState _activeState;

    private readonly StateFactory _stateFactory;

    public GameStateMachine(StateFactory stateFactory) => _stateFactory = stateFactory;

    public void Initialize()
    {
      _states = new Dictionary<Type, IGameExitableState>()
      {
        [typeof(BootStrapperState)] = _stateFactory.CreateState<BootStrapperState>(),
        [typeof(LoadProgressState)] = _stateFactory.CreateState<LoadProgressState>(),
        [typeof(MainMenuState)] = _stateFactory.CreateState<MainMenuState>(),
        [typeof(LoadLevelState)] = _stateFactory.CreateState<LoadLevelState>(),
        [typeof(GameLoopState)] = _stateFactory.CreateState<GameLoopState>()
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
