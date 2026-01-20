// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Gameplay.Services.AssetManagement
{
  public interface IGameFactory
  {
    /// <summary>
    /// Creates a hero and places it at the Vector3.zero world coordinates.
    /// </summary>
    /// <returns>GameObject</returns>
    public GameObject CreateHero();
    /// <summary>
    /// Creates a hero and places it at the PlayerStart object.
    /// Hero will be facing the same way the arrow in PlayerStart does.
    /// </summary>
    /// <returns>GameObject</returns>
    public GameObject CreateAndPlaceHero();
    /// <summary>
    /// Creates base HUD class and adds to the scene.
    /// </summary>
    /// <returns></returns>
    public GameObject CreateHud();
  }
}
