// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

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

    public IBuffDataSubservice BuffData { get; private set; }

    public StaticDataService(
      IBuildConfigSubservice buildConfig,
      IGameConfigSubservice gameConfig,
      IPlayerDataSubervice playerData,
      IEnemyDataSubservice enemyData,
      ILevelDataSubservice levelData,
      IWindowDataSubservice windowData,
      IBuffDataSubservice buffData
      )
    {
      BuildConfig = buildConfig;
      GameConfig = gameConfig;

      PlayerData = playerData;
      EnemyData = enemyData;
      LevelData = levelData;
      WindowData = windowData;

      BuffData = buffData;
    }

    public async UniTask LoadBuildDataAsync() =>
      await BuildConfig.LoadSelfAsync();

    public async UniTask LoadGameDataAsync()
    {
      await PlayerData.LoadSelfAsync();

      await GameConfig.LoadSelfAsync();

      await EnemyData.LoadSelfAsync();
      await LevelData.LoadSelfAsync();
      await WindowData.LoadSelfAsync();

      await BuffData.LoadSelfAsync();
    }
  }
}
