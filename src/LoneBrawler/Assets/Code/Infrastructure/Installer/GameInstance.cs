// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Infrastructure.Installer.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.StateMachine;
using Code.Infrastructure.StateMachine.States;

using Cysharp.Threading.Tasks;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Infrastructure.Installer
{
  public class GameInstance : ZenjexBehaviour, ICoroutineRunner
  {
    public static GameInstance Instance { get; private set; }

    [Zenjex] private readonly IStaticDataService _staticData;
    [Zenjex] private readonly GameStateMachine _stateMachine;

    protected override void OnAwake()
    {
      base.OnAwake();
      RegisterSingletone();
    }

    public async void LaunchGame()
    {
      await InitializeGameInstanceComponents();
      StartGame();
    }

    private async UniTask InitializeGameInstanceComponents()
    {
      await _staticData.LoadBuildDataAsync();

      foreach (var component in GetComponents<IGameInstanceComponent>())
        component.DelayedAwake();
    }

    private void StartGame() => _stateMachine.EnterState<BootStrapperState>();

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
