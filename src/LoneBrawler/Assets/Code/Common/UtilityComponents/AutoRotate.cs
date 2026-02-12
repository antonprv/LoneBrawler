// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Services.Time;

using UnityEngine;


namespace Code.Gameplay.Utils
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
