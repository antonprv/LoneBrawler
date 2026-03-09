// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.StaticData;
using Code.Data.StaticData.Types.UI;
using Code.Gameplay.Audio.Music;
using Code.Gameplay.Audio.Music.Interfaces;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.UI.Services.WindowService.Interfaces;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.CreditsRoll
{
  [RequireComponent(typeof(Collider))]
	public class CreditsRollTrigger : ZenjexBehaviour
	{
    public MusicPlaylist winPlaylist;

    [Zenjex] private readonly IWindowService _windowService;
    [Zenjex] private readonly IGameConfigSubservice _gameConfig;
    [Zenjex] private readonly IInputService _inputService;
    [Zenjex] private readonly IMusicPlayerHolder _musicPlayerHolder;

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

      ShowCreditsRoll();
      DisableGameInput();
      PlayWinMusic();
    }

    private void PlayWinMusic()
    {
      _musicPlayerHolder.Current.Stop();
      _musicPlayerHolder.Current.SetPlaylist(winPlaylist);
      _musicPlayerHolder.Current.Play();
    }

    private void DisableGameInput()
    {
      _inputService.GameInputEnabled = false;
    }

    private void ShowCreditsRoll()
    {
      _windowService.Open(WindowTypeId.Credits, null);
    }

    private bool IsPlayer(Collider other) =>
      ((1 << other.gameObject.layer) & _playerLayer) != 0;
  }
}
