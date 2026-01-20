using Code.Gameplay.Features.GameplayCamera;

using UnityEngine;

namespace Code.Gameplay.Services.AssetManagement
{
  public interface ICameraManager
  {
    public void Follow(GameObject objectToFollow);
  }
}
