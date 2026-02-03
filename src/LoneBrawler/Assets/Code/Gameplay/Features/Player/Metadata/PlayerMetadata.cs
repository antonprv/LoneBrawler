// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Player.Metadata.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Gameplay.Features.Player.Metadata
{
  public class PlayerMetadata : MonoBehaviour, IPlayerMetadata
  {
    public void Construct(IGameConfigSubservice gameConfigSubservice)
    {
      gameObject.tag = gameConfigSubservice.PlayerTag;
      gameObject.layer = gameConfigSubservice.PlayerLayer;
    }
  }
}
