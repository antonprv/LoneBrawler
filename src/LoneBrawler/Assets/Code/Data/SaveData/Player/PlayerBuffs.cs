// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.CustomTypes.Domain.Collections;
using Code.Common.CustomTypes.Domain.VectorTypes.Interfaces;
using Code.Gameplay.Features.Player.PlayerBuffs.Buffs;
using Code.Gameplay.Features.Player.PlayerBuffs.Interfaces;

namespace Code.Data.SaveData.Player
{
  [Serializable]
  public class PlayerBuffs : IValidatableData
  {
    public DictionaryData<Type, BuffBase> CollectedBuffs;

    public PlayerBuffs() =>
      CollectedBuffs = new DictionaryData<Type, BuffBase>();

    public bool IsValid() =>
      CollectedBuffs != null && CollectedBuffs.Count > 0;
  }
}
