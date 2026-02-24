// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Interfaces
{
  /// <summary>
  /// Strategy for a specific attack preset.
  /// Implementations: MeleeAttackBehaviour, RangedAttackBehaviour, etc.
  /// Created and injected by the factory; EnemyAttack only calls the interface — it knows nothing about the concrete type.
  /// </summary>
  public interface IAttackBehaviour
  {
    /// <summary>Called once after the enemy is created.</summary>
    void Initialize(
      Transform owner,
      AttackPresetStaticData preset,
      IHealth playerHealth,
      int playerLayerMask);

    /// <summary>Called by the animator / EnemyAttack at the moment the hit lands.</summary>
    void PerformHit();

    /// <summary>Called when a projectile is spawned or a cast effect fires (ranged attacks).</summary>
    void OnCast();

    /// <summary>Called when the attack is fully complete (for state cleanup).</summary>
    void OnAttackEnded();
  }
}
