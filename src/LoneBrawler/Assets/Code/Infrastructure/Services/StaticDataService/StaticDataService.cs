// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

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

    public StaticDataService(
      IBuildConfigSubservice buildConfig,
      IGameConfigSubservice gameConfig,
      IPlayerDataSubervice playerData,
      IEnemyDataSubservice enemyData,
      ILevelDataSubservice levelData,
      IWindowDataSubservice windowData
      )
    {
      BuildConfig = buildConfig;
      GameConfig = gameConfig;

      PlayerData = playerData;
      EnemyData = enemyData;
      LevelData = levelData;
      WindowData = windowData;
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
