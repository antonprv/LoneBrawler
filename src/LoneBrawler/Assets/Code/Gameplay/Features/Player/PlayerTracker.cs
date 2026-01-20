// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data;
using Code.Infrastructure.Services.PersistentProgress;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Gameplay.Features.Player
{
  public class PlayerTracker : MonoBehaviour, ISavedProgressWriter, ISavedProgressReader
  {
    private CharacterController _characterController;

    private void Awake()
    {
      _characterController = GetComponent<CharacterController>();
    }

    public void UpdateProgress(PlayerProgress playerProgress) =>
      playerProgress.WorldData.TransformOnLevel =
        new TransformOnLevel(transform.AsTransformData(), CurrentScene());


    public void LoadProgress(PlayerProgress playerProgress)
    {
      if (CurrentScene() == playerProgress.WorldData.TransformOnLevel.LevelName)
      {
        TransformData savedTransform = playerProgress.WorldData.TransformOnLevel.Transform;
        if (savedTransform != null)
        {
          Warp(to: savedTransform);
        }
      }
    }

    private void Warp(TransformData to)
    {
      _characterController.enabled = false;
      to.ApplyTo(transform);
      _characterController.enabled = true;
    }

    private static string CurrentScene() => SceneManager.GetActiveScene().name;
  }
}
