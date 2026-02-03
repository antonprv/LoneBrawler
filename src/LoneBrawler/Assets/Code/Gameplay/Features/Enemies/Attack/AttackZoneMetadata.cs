// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Attack.Interfaces;

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack
{
  public class AttackZoneMetadata : MonoBehaviour, IAttackZoneMetadata
  {
    public void Construct(IGameConfigSubservice gameConfig) =>
      gameObject.layer = gameConfig.AttackZoneLayer;
  }
}
