// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Features.Buffs;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.Factory.Interfaces
{
  public interface IBuffFactory
  {
    /// <summary>
    /// Creates and returns a buff instance.
    /// Registration in tracker and calling Activate() is responsibility of caller side.
    /// </summary>
    public UniTask<BuffBase> CreateBuff(BuffClassName buffClass, GameObject buffOwner);
  }
}
