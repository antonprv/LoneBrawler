// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data;
using Code.Infrastructure.Services.PersistentProgress;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Gameplay.Features.Player
{
  public class PlayerTracker : MonoBehaviour, ISavedProgressWriter
  {
    public void UpdateProgress(PlayerProgress playerProgress) =>
      playerProgress.WorldData.TransformOnLevel =
        new TransformOnLevel(transform.AsTransformData(), CurrentScene());

    private static string CurrentScene() => SceneManager.GetActiveScene().name;
  }
}
