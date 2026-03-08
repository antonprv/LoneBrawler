// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Vfx.Interfaces
{
  public interface IVfxPool
  {
    /// <summary>Retrieve a VFX instance from the pool, activate it at the given position/rotation.</summary>
    GameObject Get(Vector3 position, Quaternion rotation);
    void Return(VfxInstance instance);
  }
}
