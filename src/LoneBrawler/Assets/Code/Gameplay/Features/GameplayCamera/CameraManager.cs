// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;

using UnityEngine;

namespace Code.Gameplay.Features.GameplayCamera
{
  public class CameraManager : ICameraManager
  {
    private readonly IGameLog _logging;

    public CameraManager()
    {
      _logging = RootContext.Resolve<IGameLog>();
    }

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
