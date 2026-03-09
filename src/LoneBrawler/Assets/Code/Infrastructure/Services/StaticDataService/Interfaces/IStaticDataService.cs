// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.Services.StaticDataService.Interfaces
{
  public interface IStaticDataService
  {
    public IBuildConfigSubservice BuildConfig { get; }
    public IGameConfigSubservice GameConfig { get; }
    public IInventoryConfigSubservice InventoryConfig { get; }

    public IPlayerDataSubervice PlayerData { get; }
    public IEnemyDataSubservice EnemyData { get; }
    public ILevelDataSubservice LevelData { get; }
    public IWindowDataSubservice WindowData { get; }
    public IBuffDataSubservice BuffData { get; }
    ILevelMusicDataSubservice LevelMusic { get; }
    IMusicConfigSubservice MusicConfig { get; }

    public UniTask LoadBuildDataAsync();

    public UniTask LoadInventoryConfigAsync();

    public UniTask LoadGameDataAsync();
    UniTask LoadMusicConfigAsync();
  }
}
