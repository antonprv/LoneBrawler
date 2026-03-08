// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;

using R3;

namespace Code.Gameplay.Features.Loot.Interfaces
{
  public interface ILoot
  {
    int Souls { get; set; }

    Observable<Unit> OnCollected { get; }

    void Construct(EnemyStaticData enemyData);
  }
}
