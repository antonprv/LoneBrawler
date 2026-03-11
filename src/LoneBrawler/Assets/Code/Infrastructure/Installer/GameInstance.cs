// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

using UnityEngine;

namespace Code.Infrastructure.Installer
{
  [DefaultExecutionOrder(-10)]
  public class GameInstance : ZenjexBehaviour, ICoroutineRunner
  {
    public static GameInstance Instance { get; private set; }

    [Zenjex] private readonly IStaticDataService _staticData;
    [Zenjex] private readonly IGameStateMachine _stateMachine;

    protected override void OnAwake()
    {
      base.OnAwake();
      RegisterSingletone();
    }

    public async void LaunchGame()
    {
      await _staticData.LoadBuildDataAsync();
      StartGame();
    }

    private void StartGame() =>
      _stateMachine.EnterState<BootStrapperState>();

    private void RegisterSingletone()
    {
      DontDestroyOnLoad(this);
      if (Instance != null && Instance != this)
      {
        Destroy(gameObject);
        return;
      }

      Instance = this;
    }
  }
}
