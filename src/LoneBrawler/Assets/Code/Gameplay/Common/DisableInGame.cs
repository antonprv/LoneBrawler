// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Gameplay.Common
{
  public class DisableInGame : MonoBehaviour
  {
    private void Awake()
    {
      this.gameObject.SetActive(false);
    }
  }
}

