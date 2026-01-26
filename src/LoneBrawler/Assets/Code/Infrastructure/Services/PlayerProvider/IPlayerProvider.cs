// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Infrastructure.Services.PlayerProvider
{
  public interface IPlayerProvider
  {
  }

  public interface IPlayerReader : IPlayerProvider
  {
    GameObject Player { get; }
  }

  internal interface IPlayerWriter : IPlayerProvider
  {
    void SetPlayer(GameObject player);
  }

}
