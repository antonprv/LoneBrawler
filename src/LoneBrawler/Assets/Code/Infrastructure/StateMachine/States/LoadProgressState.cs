// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.PersistentProgress;
using Code.Infrastructure.StateMachine;
using Code.Infrastructure.StateMachine.States;

namespace Assets.Code.Infrastructure.StateMachine.States
{
  internal class LoadProgressState : IGameState
  {
    private GameStateMachine _gameStateMachine;
    private IPersistentProgressService _progressService;

    public LoadProgressState(GameStateMachine gameStateMachine)
    {
      _gameStateMachine = gameStateMachine;
      _progressService = gameStateMachine.Dependencies.progressService;
    }

    public void Enter()
    {
      throw new System.NotImplementedException();
    }

    public void Exit()
    {
      throw new System.NotImplementedException();
    }
  }
}
