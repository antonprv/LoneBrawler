// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Services.CameraManager.Interfaces;

using UnityEngine;

namespace Code.Infrastructure.Services.CameraManager
{
  public class CameraManager : ICameraManager
  {
    private readonly IGameLog _logging;

    public CameraManager(IGameLog gameLog) => _logging = gameLog;

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
