// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.Metadata;

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Enemies.Attack
{
  public class AttackZoneMetadata : ZenjexBehaviour, IMetadata
  {
    [Zenjex] private readonly IGameConfigSubservice _gameConfig;

    public void AssignMetadata() =>
      gameObject.layer = _gameConfig.AttackZoneLayer;
  }
}
