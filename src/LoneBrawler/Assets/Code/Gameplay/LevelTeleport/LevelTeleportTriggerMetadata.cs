// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.LevelTeleport.Interfaces;

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Gameplay.LevelTeleport
{
  public class LevelTeleportTriggerMetadata : MonoBehaviour, ILevelTeleportTriggerMetadata
  {
    public void Construct(IGameConfigSubservice gameConfig) =>
      gameObject.layer = gameConfig.SaveTriggerLayer;
  }
}
