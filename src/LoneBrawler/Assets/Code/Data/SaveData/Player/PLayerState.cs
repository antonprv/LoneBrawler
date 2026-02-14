// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.CustomTypes.Interfaces;

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

namespace Code.Data.SaveData.Player
{
  [System.Serializable]
  public class PLayerState : IValidatableData
  {
    public float MaxHealth;
    public float CurrentHealth;

    /// <summary>
    /// Empty constructor will always create invalid (empty) PlayerState
    /// </summary>
    public PLayerState(IPlayerDataSubervice playerStaticData)
    {
      MaxHealth = playerStaticData.MaxHealth;
      CurrentHealth = MaxHealth;
    }

    public bool IsDataNull()
    {
      return MaxHealth != 0 && CurrentHealth != 0;
    }
  }
}
