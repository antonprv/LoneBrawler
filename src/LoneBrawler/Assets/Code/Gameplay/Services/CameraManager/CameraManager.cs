using Code.Gameplay.Features.GameplayCamera;

using UnityEngine;

namespace Code.Gameplay.Services.AssetManagement
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
