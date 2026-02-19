// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using Code.Infrastructure.Installer;
using Code.Infrastructure.StateMachine.States;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Elements.Utils.LoadingScreen.Interfaces;

namespace Code.Infrastructure.StateMachine
{
  public class GameStateMachine
  {
    private Dictionary<Type, IGameExitableState> _states;
    private IGameExitableState _activeState;

    public GameStateMachine(ILoadScreen curtain)
    {
      _states = new Dictionary<Type, IGameExitableState>()
      {
        [typeof(BootStrapperState)] =
          new BootStrapperState(this),

        [typeof(LoadProgress)] =
          new LoadProgress(this),

        [typeof(LoadLevelState)] =
          new LoadLevelState(this, curtain),

        [typeof(GameLoopState)] =
          new GameLoopState(this)
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
