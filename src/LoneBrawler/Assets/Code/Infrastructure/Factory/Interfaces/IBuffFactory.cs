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
    /// Создаёт и возвращает экземпляр баффа.
    /// Регистрация в трекере и вызов Activate() — ответственность вызывающей стороны.
    /// </summary>
    public UniTask<BuffBase> CreateBuff(BuffClassName buffClass, GameObject buffOwner);
  }
}
