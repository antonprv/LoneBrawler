// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Save
{
  public class SaveTrigger : ZenjexBehaviour
  {
    public BoxCollider BoxCollider;
    [Zenjex] private readonly IGameLog _logging;
    [Zenjex] private readonly ISaveLoadService _saveLoadService;
    [Zenjex] private readonly IGameConfigSubservice _gameConfig;

    private int _playerLayer;
    private bool _collided;

    protected override void OnAwake()
    {
      base.OnAwake();
      _playerLayer = _gameConfig.PlayerLayerBitmask;
    }

    private void OnTriggerEnter(Collider other)
    {
      if (!IsPlayer(other)) return;

      if (_collided) return;
      _collided = true;

      _saveLoadService.SaveProgress();
      _logging.Log("GameSaved");
      gameObject.SetActive(false);
    }

    private bool IsPlayer(Collider other) =>
      ((1 << other.gameObject.layer) & _playerLayer) != 0;
  }
}
