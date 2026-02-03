// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Aggro.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Aggro
{
  public class AggroMetadata : MonoBehaviour, IAggroMetadata
  {
    public void Construct(IGameConfigSubservice gameConfig) =>
      gameObject.layer = gameConfig.AggroLayer;
  }
}
