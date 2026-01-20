// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Gameplay.Features.GameplayCamera
{
  public class CameraManager : ICameraManager
  {
    private readonly Camera _camera;

    public CameraManager()
    {
      _camera = Camera.main;
    }

    public void Follow(GameObject objectToFollow) =>
      _camera.GetComponent<CameraFollow>().Follow(objectToFollow);
  }
}
