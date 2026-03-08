// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Audio.Music;
using Code.Gameplay.Audio.Music.Interfaces;

using Reflex.Core;

using Zenjex.Extensions.Core;
using Zenjex.Extensions.SceneContext;

public class GameplaySceneInstaller : SceneInstaller
{
  public override void InstallBindings(ContainerBuilder builder)
  {
    builder.Bind<IMusicPlayer>()
           .To<MusicPlayer>()
           .FromComponentInHierarchy()
           .AsSingle();
  }
}
