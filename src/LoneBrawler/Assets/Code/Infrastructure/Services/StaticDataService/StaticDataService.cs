// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

namespace Code.Infrastructure.Services.StaticDataService
{
  public class StaticDataService : IStaticDataService
  {
    public IBuildConfigSubservice BuildConfig { get; private set; }
    public IGameConfigSubservice GameConfig { get; private set; }
    public IPlayerDataSubervice PlayerData { get; private set; }
    public IEnemyDataSubservice EnemyData { get; private set; }

    public StaticDataService()
    {
      BuildConfig = RootContext.Resolve<IBuildConfigSubservice>();
      GameConfig = RootContext.Resolve<IGameConfigSubservice>();
      PlayerData = RootContext.Resolve<IPlayerDataSubervice>();
      EnemyData = RootContext.Resolve<IEnemyDataSubservice>();
    }
  }
}
