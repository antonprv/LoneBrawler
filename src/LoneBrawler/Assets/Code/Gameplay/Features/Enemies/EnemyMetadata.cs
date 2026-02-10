// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies
{
  public class EnemyMetadata : MonoBehaviour
  {
    public void Construct(IGameConfigSubservice gameConfig) =>
      gameObject.tag = gameConfig.EnemyTag;
  }
}
