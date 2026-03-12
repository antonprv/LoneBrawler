// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

#region Service Includes

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Gameplay.Audio.Music;
using Code.Gameplay.Audio.Music.Interfaces;
using Code.Gameplay.Utils.Visuals;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.SceneLoader.Interfaces;
using Code.Infrastructure.Services.BuffService;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.CameraManager;
using Code.Infrastructure.Services.CameraManager.Interfaces;
using Code.Infrastructure.Services.Input;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerPrefs;
using Code.Infrastructure.Services.PlayerPrefs.Interfaces;
using Code.Infrastructure.Services.PlayerProvider;
using Code.Infrastructure.Services.Random;
using Code.Infrastructure.Services.RestartGame;
using Code.Infrastructure.Services.RestartGame.Interfaces;
using Code.Infrastructure.Services.SaveLoad;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.SoulsTracker;
using Code.Infrastructure.Services.SoulsTracker.Interfaces;
using Code.Infrastructure.Services.SoundService;
using Code.Infrastructure.Services.SoundService.Interfaces;
using Code.Infrastructure.Services.StaticDataService;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.StaticDataService.Subservices;
using Code.Infrastructure.Services.Time;
using Code.Infrastructure.StateMachine;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;
using Code.Infrastructure.StateMachine.Factory;
using Code.UI.Elements.Common.LoadingScreen.Interfaces;
using Code.UI.Factory;
using Code.UI.Factory.Interfaces;
using Code.UI.Services.DragDropService;
using Code.UI.Services.DragDropService.Interfaces;
using Code.UI.Services.DragIcon;
using Code.UI.Services.InventoryService;
using Code.UI.Services.InventoryService.Interfaces;
using Code.UI.Services.TooltipService;
using Code.UI.Services.WindowService;
using Code.UI.Services.WindowService.Interfaces;

#endregion

using Reflex.Core;

using UnityEngine;

using Zenjex.Extensions.Core;

using Code.Infrastructure.DevConsole.Interfaces;
using Code.Infrastructure.DevConsole.Service;
using Code.Infrastructure.DevConsole;

namespace Code.Infrastructure.Installer
{
  public class GameInstaller : ProjectRootInstaller
  {
    private GameInstance _gameInstance;
    private ILoadScreen _loadScreen;

    public override IEnumerator InstallGameInstanceRoutine()
    {
      yield return InstallerFactory.CreateLoadingScreenRoutine(screen =>
          _loadScreen = screen);

      RootContainer.Bind<ILoadScreen>()
        .FromInstance(_loadScreen)
        .AsSingle();

      yield return InstallerFactory.CreateGameInstanceRoutine(
      onBeforeActivate: instance =>
      {
        _gameInstance = instance;
        BindGameInstanceComponents(instance);
      });
    }

    private static void BindGameInstanceComponents(GameInstance instance)
    {
      RootContainer.Bind<ICoroutineRunner>()
        .FromInstance(instance)
        .AsSingle();

      RootContainer.Bind<ILiveProgressSync>()
        .FromInstance(instance.GetComponent<ILiveProgressSync>())
        .AsSingle();

      RootContainer.Bind<FramerateManager>()
        .FromInstance(instance.GetComponent<FramerateManager>())
        .AsSingle();

      RootContainer.Bind<IConsoleComponent>()
        .FromInstance(instance.GetComponent<IConsoleComponent>())
        .AsSingle();
    }

    public override void InstallBindings(ContainerBuilder builder)
    {
      // Game State Machine
      BindStateMachine(builder);

      // Domain | Asset management
      BindLogging(builder);
      BindSceneLoader(builder);
      BindAssetManagement(builder);

      // Gameplay | Baseline unity services
      BindCameraManager(builder);
      BindInputService(builder);
      BindUnityServices(builder);

      // Progress and data
      BindProgressServices(builder);
      BindPlayerPrefsService(builder);
      BindStaticData(builder);
      BindResetProgressService(builder);

      // Gameplay-only services
      BindPlayerProvider(builder);
      BindSoulsTracker(builder);

      // UI | UX
      BindUI(builder);
      BindDevConsole(builder);

      // All factories and factory methods
      BindFactory(builder);

      // Buff system (purchase\activate\keep)
      BindGameplayBuffs(builder);
      BindInventory(builder);

      // Game Audio
      BindSoundService(builder);
      BindMusicServices(builder);
    }

    public override void LaunchGame() => _gameInstance.LaunchGame();

    #region Game State Machine

    private void BindStateMachine(ContainerBuilder builder)
    {
      builder.Bind<BootStrapperState>().AsSingle();
      builder.Bind<LoadProgressState>().AsSingle();
      builder.Bind<MainMenuState>().AsSingle();
      builder.Bind<LoadLevelState>().AsSingle();
      builder.Bind<GameLoopState>().AsSingle();

      builder.Bind<StateFactory>().AsSingle();

      builder.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();
    }

    #endregion

    #region Domain | Asset management

    private void BindLogging(ContainerBuilder builder) =>
    builder.Bind<IGameLog>().To<GameLogger>().AsSingle();

    private void BindSceneLoader(ContainerBuilder builder) =>
    builder.Bind<ISceneLoader>().To<AddressableSceneLoader>().AsSingle();

    private void BindAssetManagement(ContainerBuilder builder) =>
    builder.Bind<IAssetLoader>().To<AssetLoader>().AsSingle();

    #endregion

    #region Gameplay | Baseline unity services

    private void BindCameraManager(ContainerBuilder builder) =>
    builder.Bind<ICameraManager>().To<CameraManager>().AsSingle();

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
    private void BindUnityServices(ContainerBuilder builder)
    {
      builder.Bind<ITimeService>().To<UnityTimeService>().AsSingle();
      builder.Bind<IRandomService>().To<UnityRandomService>().AsSingle();
    }

    #endregion

    #region Progress and data

    private void BindProgressServices(ContainerBuilder builder)
    {
      builder.Bind<IPersistentProgressService>().To<PersistentProgressService>().AsSingle();
      builder.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle();
    }

    private void BindPlayerPrefsService(ContainerBuilder builder) =>
      builder.Bind<IPlayerPrefsService>().To<PlayerPrefsService>().AsSingle();

    private void BindStaticData(ContainerBuilder builder)
    {
      builder.Bind<IBuildConfigSubservice>().To<BuildConfigSubservice>().AsSingle();
      builder.Bind<IGameConfigSubservice>().To<GameConfigSubservice>().AsSingle();
      builder.Bind<IInventoryConfigSubservice>().To<InventoryConfigSubservice>().AsSingle();
      builder.Bind<IMusicConfigSubservice>().To<MusicConfigSubservice>().AsSingle();

      builder.Bind<IPlayerDataSubervice>().To<PlayerDataSubservice>().AsSingle();
      builder.Bind<IEnemyDataSubservice>().To<EnemyDataSubservice>().AsSingle();
      builder.Bind<ILevelDataSubservice>().To<LevelDataSubservice>().AsSingle();
      builder.Bind<IWindowDataSubservice>().To<WindowDataSubservice>().AsSingle();
      builder.Bind<IBuffDataSubservice>().To<BuffDataSubservice>().AsSingle();
      builder.Bind<ILevelMusicDataSubservice>().To<LevelMusicDataSubservice>().AsSingle();

      builder.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
    }

    private void BindResetProgressService(ContainerBuilder builder) =>
      builder.Bind<IRestartGameService>().To<RestartGameService>().AsSingle();

    #endregion

    #region Gameplay-only services

    private void BindPlayerProvider(ContainerBuilder builder) =>
      builder.Bind<PlayerProvider>().BindInterfaces().AsSingle();

    private void BindSoulsTracker(ContainerBuilder builder) =>
    builder.Bind<ISoulsTrackerService>().To<SoulsTrackerService>().AsSingle();

    #endregion

    #region UI | UX

    private void BindUI(ContainerBuilder builder)
    {
      builder.Bind<IUIFactory>().To<UIFactory>().AsSingle();
      builder.Bind<IWindowService>().To<WindowService>().AsSingle();
      builder.Bind<IDragDropService>().To<DragDropService>().AsSingle();
    }

    private void BindDevConsole(ContainerBuilder builder) =>
      builder.Bind<IDevConsole>().To<DevConsoleService>().AsSingle();

    #endregion

    #region All factories and factory methods

    private void BindFactory(ContainerBuilder builder)
    {
      builder.Bind<IShopItemFactory>().To<ShopItemFactory>().AsSingle();
      builder.Bind<IAttackBehaviourFactory>().To<AttackBehaviourFactory>().AsSingle();
      builder.Bind<IGameFactory>().To<GameFactory>().AsSingle();
    }

    #endregion

    #region Buff system (purchase\activate\keep)

    private void BindGameplayBuffs(ContainerBuilder builder)
    {
      builder.Bind<IBuffTrackerService>().To<BuffTrackerService>().AsSingle();
      builder.Bind<IBuffFactory>().To<BuffFactory>().AsSingle();
    }

    private void BindInventory(ContainerBuilder builder)
    {
      builder.Bind<TooltipService>().BindInterfaces().AsSingle();
      builder.Bind<DragIconService>().BindInterfaces().AsSingle();
      builder.Bind<IInventoryFactory>().To<InventoryFactory>().AsSingle();
      builder.Bind<IInventoryService>().To<InventoryService>().AsSingle();
    }

    #endregion

    #region Game Audio

    private void BindSoundService(ContainerBuilder builder) =>
      builder.Bind<ISoundService>().To<SoundService>().AsSingle();

    private void BindMusicServices(ContainerBuilder builder)
    {
      builder.Bind<ITrackLoader>().To<AddressableTrackLoader>().AsSingle();
      builder.Bind<IFader>().To<AudioFader>().AsSingle();
      builder.Bind<IMusicPlayerHolder>().To<MusicPlayerHolder>().AsSingle();
    }

    #endregion
  }
}
