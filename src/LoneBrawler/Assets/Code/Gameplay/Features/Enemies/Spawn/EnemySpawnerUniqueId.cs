// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Gameplay.Common.NPCInterfaces.Utils;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Spawn
{
  public class EnemySpawnerUniqueId : MonoBehaviour, IIdContainer
  {
    public string Id { get; set; }

    public GameObject GameObject => gameObject;

    public Component Self => this;
  }
}
