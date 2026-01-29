// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Services.StaticDataService.Interfaces;

namespace Code.Infrastructure.Services.StaticDataService
{
  public class StaticDataService : IStaticDataService
  {
    public IGameConfigSubservice GameConfig { get; private set; }
    public IPlayerDataSubervice PlayerData { get; private set; }
    public IEnemyDataSubservice EnemyData { get; private set; }

    public StaticDataService()
    {
      GameConfig = RootContext.Resolve<IGameConfigSubservice>();
      PlayerData = RootContext.Resolve<IPlayerDataSubervice>();
      EnemyData = RootContext.Resolve<IEnemyDataSubservice>();
    }
  }
}
