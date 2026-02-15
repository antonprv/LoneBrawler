// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

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

    public async Task LoadBuildDataAsync() =>
      await BuildConfig.LoadSelfAsync();

    public async Task LoadGameDataAsync()
    {
      await PlayerData.LoadSelfAsync();

      await GameConfig.LoadSelfAsync();

      await EnemyData.LoadSelfAsync();
      await LevelData.LoadSelfAsync();
      await WindowData.LoadSelfAsync();
    }
  }
}
