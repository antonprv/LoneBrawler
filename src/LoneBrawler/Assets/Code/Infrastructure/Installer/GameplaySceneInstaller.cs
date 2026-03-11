// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Audio.Music;
using Code.Gameplay.Audio.Music.Interfaces;

using Reflex.Core;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Core;
using Zenjex.Extensions.SceneContext;

namespace Code.Infrastructure.Installer
{
  public class GameplaySceneInstaller : SceneInstaller
  {
    [Zenjex] private readonly IMusicPlayerHolder _holder;

    public override void InstallBindings(ContainerBuilder builder)
    {
      builder.Bind<IMusicPlayer>()
             .To<MusicPlayer>()
             .FromComponentInHierarchy()
             .AsSingle();
    }

    protected override void OnInstalled()
    {
      base.OnInstalled();
      _holder.Register(SceneContainer.Resolve<IMusicPlayer>());
    }
     
    private void OnDestroy() => _holder.Unregister();
  }
}
