// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;

using Reflex.Attributes;

using UnityEngine;

namespace Code.Gameplay.Features.GameplayCamera
{
  public class CameraManager : ICameraManager
  {
    [Inject] private IGameLog _logging;

    public void Follow(GameObject objectToFollow)
    {
      CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
      if (cameraFollow == null)
      {
        _logging.Log(LogType.Error, "Unable to find CameraFollow component on main camera");
        return;
      }
      cameraFollow.Follow(objectToFollow);
    }
  }
}
