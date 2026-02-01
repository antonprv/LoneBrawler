// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Gameplay.Common.NPCInterfaces.Utils
{
  public interface IIdContainer
  {
    public string Id { get; set; }

    public GameObject GameObject { get; }

    public Component Self { get; }
  }
}
