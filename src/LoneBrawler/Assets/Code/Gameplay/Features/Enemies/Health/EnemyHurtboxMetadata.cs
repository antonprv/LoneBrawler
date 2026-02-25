// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.Metadata;
using Code.Gameplay.Features.Enemies.Health.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Enemies.Health
{
  public class EnemyHurtboxMetadata : ZenjexBehaviour, IMetadata
  {
    [Zenjex] private readonly IGameConfigSubservice _gameConfig;

    public void AssignMetadata() =>
      gameObject.layer = _gameConfig.EnemyHitableLayer;
  }
}
