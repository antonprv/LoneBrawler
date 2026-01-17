// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Code.Infrastructure
{
  public class GameStateMachine
  {
    private Dictionary<Type, IGameState> _states;
    private IGameState _activeState;

    public GameStateMachine()
    {
      _states = new Dictionary<Type, IGameState>()
      {
        [typeof(BootStrapperState)] = new BootStrapperState(this)
      };
    }

    public void EnterState<TState>() where TState : IGameState
    {
      _activeState?.Exit();
      IGameState gameState = _states[typeof(TState)];
      _activeState = gameState;
      gameState.Enter();
    }
  }
}
