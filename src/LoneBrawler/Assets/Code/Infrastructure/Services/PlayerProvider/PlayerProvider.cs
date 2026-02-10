// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Utils.Extensions.Logging;
using Code.Utils.Extensions.ReflexExtensions;

using UnityEngine;

namespace Code.Infrastructure.Services.PlayerProvider
{
  public class PlayerProvider : IPlayerReader, IPlayerWriter
  {
    private IGameLog _logger;

    GameObject _player;

    public PlayerProvider()
    {
      _logger = RootContext.Resolve<IGameLog>();
    }

    public GameObject GetPlayer()
    {
      if (_player == null)
      {
        return null;
      }
      return _player;
    }

    public void SetPlayer(GameObject player)
    {
      if (_player != null)
      {
        _logger.Log(LogType.Warning,
          "Trying to set player again somewhere in code.");
      }

      _player = player;
    }
  }
}
