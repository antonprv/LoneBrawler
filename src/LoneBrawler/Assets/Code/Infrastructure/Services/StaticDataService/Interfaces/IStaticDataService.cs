// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.Services.StaticDataService.Interfaces
{
  public interface IStaticDataService
  {
    public IBuildConfigSubservice BuildConfig { get; }
    public IGameConfigSubservice GameConfig { get; }
    public IPlayerDataSubervice PlayerData { get; }
    public IEnemyDataSubservice EnemyData { get; }
  }
}
