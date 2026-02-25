// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Data.StaticData.Types.Attack
{
  /// <summary>
  /// Defines how an attack hits targets: single or area.
  /// Independent from EnemyAttackType (melee/ranged).
  /// Examples: melee AoE — jump slam with a hammer; ranged AoE — exploding fireball.
  /// </summary>
  public enum AttackTargetMode
  {
    None = 0,
    SingleTarget = 1,
    AreaOfEffect = 2,
  }
}
