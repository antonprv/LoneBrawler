// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Infrastructure.Installer.Interfaces;
using Code.Infrastructure.StateMachine;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;
using Code.Infrastructure.StateMachine.States.Interfaces;
using Code.UI.Elements.Common.LoadingScreen.Interfaces;

using UnityEngine;

namespace Code.Infrastructure.Installer
{
  public class GameInstance : MonoBehaviour, ICoroutineRunner, IGameStateMachine
  {
    public static GameInstance Instance { get; private set; }

    private GameStateMachine _stateMachine;
    private ILoadScreen _loadScreen;

    public void Construct(ILoadScreen loadScreen)
    {
      RegisterSingletone();
      _loadScreen = loadScreen;
    }

    public void LaunchGame()
    {
      InitializeGameInstanceComponents();
      InitializeStateMachine();
      StartGame();
    }

    private void InitializeGameInstanceComponents()
    {
      foreach (var component in GetComponents<IGameInstanceComponent>())
      {
        component.RegisterGameInstance(this);
        component.DelayedAwake();
      }
    }

    private void InitializeStateMachine() =>
      _stateMachine = new GameStateMachine(this, _loadScreen);

    private void StartGame() => _stateMachine.EnterState<BootStrapperState>();

    private void RegisterSingletone()
    {
      DontDestroyOnLoad(this);
      if (Instance != null && Instance != this)
      {
        Destroy(gameObject);
        return;
      }
    }

    #region GameStateMachine interface
    public void EnterState<TState, TPayload>(TPayload payload)
      where TState : class, IGamePayloadedState<TPayload> =>
      _stateMachine.EnterState<TState, TPayload>(payload);

    public void EnterState<TState>()
      where TState : class, IGameState =>
      _stateMachine?.EnterState<TState>();
    #endregion
  }
}
