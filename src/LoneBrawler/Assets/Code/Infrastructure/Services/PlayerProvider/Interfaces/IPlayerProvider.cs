// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Infrastructure.Services.PlayerProvider.Interfaces
{
  public interface IPlayerProvider
  {
  }

  public interface IPlayerReader : IPlayerProvider
  {
    public GameObject GetPlayer();
  }

  public interface IPlayerWriter : IPlayerProvider
  {
    void SetPlayer(GameObject player);
  }

}
