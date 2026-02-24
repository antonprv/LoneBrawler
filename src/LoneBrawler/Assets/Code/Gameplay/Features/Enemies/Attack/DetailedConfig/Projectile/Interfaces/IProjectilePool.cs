// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Projectile.Interfaces
{
  public interface IProjectilePool
  {
    /// <summary>Retrieve a projectile from the pool. Activates the object and returns it.</summary>
    EnemyProjectile Get(Vector3 position, Quaternion rotation);

    /// <summary>Return a projectile to the pool. Deactivates the object.</summary>
    void Return(EnemyProjectile projectile);
  }
}
