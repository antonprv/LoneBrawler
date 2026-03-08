// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Vfx.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Vfx
{
  /// <summary>
  /// Pool for a single VFX prefab type.
  /// Mirrors the structure of ProjectilePool.
  ///
  /// Growth strategy: if the pool is exhausted a new instance is created beyond
  /// the initial size and returned when done (pool grows organically, never throws).
  /// </summary>
  public class VfxPool : IVfxPool
  {
    private const float DefaultDuration = 2f;

    private readonly GameObject _prefab;
    private readonly Transform _root;
    private readonly VfxInstance[] _pool;
    private readonly float _duration;
    private int _nextFree;

    public VfxPool(GameObject prefab, int initialSize, float duration = DefaultDuration)
    {
      _prefab = prefab;
      _duration = duration;

      _root = new GameObject($"[VfxPool] {prefab.name}").transform;
      Object.DontDestroyOnLoad(_root.gameObject);

      _pool = new VfxInstance[initialSize];
      for (int i = 0; i < initialSize; i++)
        _pool[i] = CreateInstance();
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
      VfxInstance instance = FindFree() ?? CreateInstance();

      instance.transform.SetPositionAndRotation(position, rotation);
      instance.gameObject.SetActive(true);
      instance.Play(this, _duration);

      return instance.gameObject;
    }

    public void Return(VfxInstance instance)
    {
      instance.gameObject.SetActive(false);
      instance.transform.SetParent(_root);
    }

    // ──────────────────────────────────────────────
    //  Private helpers
    // ──────────────────────────────────────────────

    private VfxInstance FindFree()
    {
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

    private VfxInstance CreateInstance()
    {
      GameObject go = Object.Instantiate(_prefab, _root);
      go.SetActive(false);

      VfxInstance instance = go.GetComponent<VfxInstance>();
      if (instance == null)
        instance = go.AddComponent<VfxInstance>();

      return instance;
    }
  }
}
