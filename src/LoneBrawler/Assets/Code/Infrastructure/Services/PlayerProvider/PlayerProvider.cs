// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using UnityEngine;

namespace Code.Infrastructure.Services.PlayerProvider
{
  public class PlayerProvider : IPlayerProvider
  {
    public void SetPlayer(GameObject player)
    {
      OnPlayerSet?.Invoke(player);
    }

    public event Action<GameObject> OnPlayerSet;

  }
}
