// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Projectile.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Projectile
{
  /// <summary>
  /// Projectile pool for a single prefab type.
  /// Created once per enemy (or per attack type if enemies share a preset).
  ///
  /// Growth strategy: if the pool is exhausted — a new object is created beyond the initial size
  /// and returned to the pool when done (pool grows organically, never throws).
  /// </summary>
  public class ProjectilePool : IProjectilePool
  {
    private readonly GameObject _prefab;
    private readonly Transform _root;   // empty container to keep the hierarchy clean
    private readonly EnemyProjectile[] _pool;
    private int _nextFree;

    public ProjectilePool(GameObject prefab, int initialSize)
    {
      _prefab = prefab;

      // Root container so pooled projectiles don't clutter the scene hierarchy
      _root = new GameObject($"[Pool] {prefab.name}").transform;
      Object.DontDestroyOnLoad(_root.gameObject);

      _pool = new EnemyProjectile[initialSize];
      for (int i = 0; i < initialSize; i++)
        _pool[i] = CreateInstance();
    }

    public EnemyProjectile Get(Vector3 position, Quaternion rotation)
    {
      EnemyProjectile projectile = FindFree() ?? CreateInstance();

      projectile.transform.SetPositionAndRotation(position, rotation);
      projectile.gameObject.SetActive(true);
      return projectile;
    }

    public void Return(EnemyProjectile projectile)
    {
      projectile.gameObject.SetActive(false);
      projectile.transform.SetParent(_root);
    }

    // ──────────────────────────────────────────────
    //  Private helpers
    // ──────────────────────────────────────────────

    private EnemyProjectile FindFree()
    {
      // Simple linear scan — pools are small (5–20 objects)
      for (int i = 0; i < _pool.Length; i++)
      {
        int idx = (_nextFree + i) % _pool.Length;
        if (!_pool[idx].gameObject.activeSelf)
        {
          _nextFree = (idx + 1) % _pool.Length;
          return _pool[idx];
        }
      }
      return null;
    }

    private EnemyProjectile CreateInstance()
    {
      GameObject go = Object.Instantiate(_prefab, _root);
      go.SetActive(false);

      var projectile = go.GetComponent<EnemyProjectile>();
      if (projectile == null)
        projectile = go.AddComponent<EnemyProjectile>();

      return projectile;
    }
  }
}
