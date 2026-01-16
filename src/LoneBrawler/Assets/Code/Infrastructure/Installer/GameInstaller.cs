// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Random;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Services.Input;

using Reflex.Core;

using UnityEngine;

public class GameInstaller : ProjectRootInstaller
{
  public override void InstallBindings(ContainerBuilder builder)
  {
    BindInputService(builder);
    BindUnityServices(builder);
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
      builder.Bind<IInputService>().To<PCInputService>().AsSingleton();
    }
    else
    {
      builder.Bind<IInputService>().To<PhoneInputService>().AsSingleton();
    }
  }
}
