// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Infrastructure.Services.CameraManager.Interfaces
{
  public interface ICameraManager
  {
    public void Follow(GameObject objectToFollow);
  }
}
