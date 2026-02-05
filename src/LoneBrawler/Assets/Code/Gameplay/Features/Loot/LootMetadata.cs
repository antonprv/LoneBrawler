// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Loot.Interfaces;

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Gameplay.Features.Loot
{
  public class LootMetadata : MonoBehaviour, ILootMetadata
  {
    public void Construct(IGameConfigSubservice gameConfig) =>
      gameObject.layer = gameConfig.LootLayer;
  }
}
