// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;

using UnityEngine;

namespace Code.Infrastructure.Services.PlayerProvider
{
  public class PlayerProvider : IPlayerReader, IPlayerWriter
  {
    private IGameLog _logger;

    public PlayerProvider()
    {
      _logger = RootContext.Resolve<IGameLog>();
    }

    public GameObject Player { get; private set; }

    public void SetPlayer(GameObject player)
    {
      if (Player != null)
      {
        _logger.Log(LogType.Warning,
          "Trying to set player again somewhere in code.");
      }

      Player = player;
    }
  }
}
