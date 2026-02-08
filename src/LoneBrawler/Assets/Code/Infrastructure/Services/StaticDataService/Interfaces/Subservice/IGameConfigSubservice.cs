// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IGameConfigSubservice
  {
    public string PlayerTag { get; }
    public string PlayerStartTag { get; }

    public int PlayerLayerBitmask { get; }
    public int EnemyHitableLayerBitmask { get; }
    public int LootLayerBitmask { get; }
    public int AggroLayerBitmask { get; }
    public int AttackZoneLayerBitmask { get; }
    public int SaveTriggerLayerBitmask { get; }

    public int PlayerLayer { get; }
    public int EnemyHitableLayer { get; }
    public int LootLayer { get; }
    public int AggroLayer { get; }
    public int AttackZoneLayer { get; }
    public int SaveTriggerLayer { get; }
  }
}
