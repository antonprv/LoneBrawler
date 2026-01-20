// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Random;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.GameplayCamera;
using Code.Infrastructure;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.Input;

using Reflex.Core;

using UnityEngine;

public class GameInstaller : ProjectRootInstaller
{
  public override void InstallBindings(ContainerBuilder builder)
  {
    BindLogging(builder);
    BindSceneLoader(builder);
    BindAssetManagement(builder);
    BindCameraManager(builder);
    BindCoroutineRunner(builder);
    BindInputService(builder);
    BindUnityServices(builder);
  }

  private void BindCoroutineRunner(ContainerBuilder builder)
  {
    builder.Bind<ICoroutineRunner>().To<GameIntance>().AsSingle();
  }

  private void BindSceneLoader(ContainerBuilder builder)
  {
    builder.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
  }
  private void BindAssetManagement(ContainerBuilder builder)
  {
    builder.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
    builder.Bind<IGameFactory>().To<GameFactory>().AsSingle();
  }

  private void BindCameraManager(ContainerBuilder builder)
  {
    builder.Bind<ICameraManager>().To<CameraManager>().AsSingle();
  }

  private void BindLogging(ContainerBuilder builder)
  {
    builder.Bind<IGameLog>().To<GameLogger>().AsSingle();
  }

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
}
