// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Time;

using UnityEngine;


namespace Code.Gameplay.Common
{
  public class AutoRotate : MonoBehaviour
  {
    private void Awake()
    {
      _timeService = RootContext.Resolve<ITimeService>();
    }

    // Rotation speed & axis
    public Vector3 rotation;

    // Rotation space
    public Space space = Space.Self;
    private ITimeService _timeService;

    void Update()
    {
      this.transform.Rotate(rotation * _timeService.DeltaAt60FPS, space);
    }
  }
}
