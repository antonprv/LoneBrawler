// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Utils.Extensions.ReflexExtensions;

namespace Code.Infrastructure.Services.StaticDataService
{
  public class StaticDataService : IStaticDataService
  {
    public IBuildConfigSubservice BuildConfig { get; private set; }
    public IGameConfigSubservice GameConfig { get; private set; }
    public IPlayerDataSubervice PlayerData { get; private set; }
    public IEnemyDataSubservice EnemyData { get; private set; }
    public ILevelDataSubservice LevelData { get; private set; }

    public IWindowDataSubservice WindowData { get; private set; }

    public StaticDataService()
    {
      BuildConfig = RootContext.Resolve<IBuildConfigSubservice>();
      GameConfig = RootContext.Resolve<IGameConfigSubservice>();

      PlayerData = RootContext.Resolve<IPlayerDataSubervice>();
      EnemyData = RootContext.Resolve<IEnemyDataSubservice>();
      LevelData = RootContext.Resolve<ILevelDataSubservice>();
      WindowData = RootContext.Resolve<IWindowDataSubservice>();
    }

    public void Load()
    {
      PlayerData.LoadSelf();
      EnemyData.LoadSelf();
      LevelData.LoadSelf();
      WindowData.LoadSelf();
    }
  }
}
