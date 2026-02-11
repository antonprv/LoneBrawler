// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Installer;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.CameraManager;
using Code.Infrastructure.Services.CameraManager.Interfaces;
using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.Input;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.LootTracker;
using Code.Infrastructure.Services.LootTracker.Interfaces;
using Code.Infrastructure.Services.PersistentProgress;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerProvider;
using Code.Infrastructure.Services.Random;
using Code.Infrastructure.Services.SaveLoad;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.StaticDataService.Subservices;
using Code.Infrastructure.Services.Time;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.UI.Factory;
using Code.UI.Factory.Interfaces;
using Code.UI.Services.WindowService;
using Code.UI.Services.WindowService.Interfaces;
using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;

using Reflex.Core;

using UnityEngine;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;

public class GameInstaller : ProjectRootInstaller
{
  private GameInstance _gameInstance;

  public override void InstallGameInstance(ContainerBuilder builder)
  {
    if (GameInstance.Instance == null)
      _gameInstance = InstallerFactory.CreateGameInstance();
    else
      _gameInstance = GameInstance.Instance;

    BindGameState(builder);
  }

  public override void InstallBindings(ContainerBuilder builder)
  {
    BindLogging(builder);
    BindSceneLoader(builder);
    BindAssetManagement(builder);
    BindCameraManager(builder);
    BindCoroutineRunner(builder);
    BindInputService(builder);
    BindUnityServices(builder);
    BindPlayerProgressServices(builder);
    BindStaticData(builder);
    BindLootTracker(builder);
    BindPlayerProvider(builder);
    BindUI(builder);
    BindDevConsole(builder);
  }

  public override void LaunchGame() => _gameInstance.LaunchGame();

  private void BindGameState(ContainerBuilder builder) =>
    builder.Bind<IGameStateMachine>()
      .FromInstance(_gameInstance)
      .BindInterfacesAndSelf()
      .AsSingle();

  private void BindDevConsole(ContainerBuilder builder) =>
    builder.Bind<IDevConsole>().To<DevConsoleService>().AsSingle();

  private void BindUI(ContainerBuilder builder)
  {
    builder.Bind<IUIFactory>().To<UIFactory>().AsSingle();
    builder.Bind<IWindowService>().To<WindowService>().AsSingle();
  }

  private void BindLootTracker(ContainerBuilder builder) =>
    builder.Bind<ILootTrackerService>().To<LootTrackerService>().AsSingle();

  private void BindStaticData(ContainerBuilder builder)
  {
    builder.Bind<IBuildConfigSubservice>().To<BuildConfigSubservice>().AsSingle();
    builder.Bind<IGameConfigSubservice>().To<GameConfigSubservice>().AsSingle();
    builder.Bind<IPlayerDataSubervice>().To<PlayerDataSubservice>().AsSingle();
    builder.Bind<IEnemyDataSubservice>().To<EnemyDataSubservice>().AsSingle();
    builder.Bind<ILevelDataSubservice>().To<LevelDataSubservice>().AsSingle();
    builder.Bind<IWindowDataSubservice>().To<WindowDataSubservice>().AsSingle();

    builder.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
  }

  private void BindCoroutineRunner(ContainerBuilder builder) =>
      builder.Bind<ICoroutineRunner>().FromInstance(_gameInstance).AsSingle();

  private void BindSceneLoader(ContainerBuilder builder) =>
    builder.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();

  private void BindAssetManagement(ContainerBuilder builder)
  {
    builder.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
    builder.Bind<IGameFactory>().To<GameFactory>().AsSingle();
  }

  private void BindCameraManager(ContainerBuilder builder) =>
    builder.Bind<ICameraManager>().To<CameraManager>().AsSingle();

  private void BindLogging(ContainerBuilder builder) =>
    builder.Bind<IGameLog>().To<GameLogger>().AsSingle();

  private void BindUnityServices(ContainerBuilder builder)
  {
    builder.Bind<ITimeService>().To<UnityTimeService>().AsSingle();
    builder.Bind<IRandomService>().To<UnityRandomService>().AsSingle();
  }

  private void BindInputService(ContainerBuilder builder)
  {
    RuntimePlatform platform = Application.platform;

    if (platform != RuntimePlatform.Android)
    {
      builder.Bind<IInputService>().To<PCInputService>().AsSingle();
    }
    else
    {
      builder.Bind<IInputService>().To<PhoneInputService>().AsSingle();
    }
  }

  private void BindPlayerProgressServices(ContainerBuilder builder)
  {
    builder.Bind<IPersistentProgressService>().To<PersistentProgressService>().AsSingle();
    builder.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle();
  }

  private void BindPlayerProvider(ContainerBuilder builder) =>
      builder.Bind<PlayerProvider>().BindInterfaces().AsSingle();
}
