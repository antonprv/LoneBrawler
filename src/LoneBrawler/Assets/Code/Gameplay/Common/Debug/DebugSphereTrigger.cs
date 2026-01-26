// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.DebugUtils;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Configs;
using Code.Gameplay.Common.Time;

using UnityEngine;

namespace Code.Gameplay.Common.Debug
{
  [RequireComponent(typeof(SphereCollider))]
  public class DebugSphereTrigger : MonoBehaviour
  {
    public SphereCollider sphereCollider;

    public Color UntriggeredColor = new Color(0.0f, 0.0f, 0.5f, 0.5f);
    public Color TriggeredColor = new Color(0.5f, 0.0f, 0.0f, 0.5f);

    private IGameLog _logging;
    private ITimeService _timeService;
    private bool _wasColldedWith = false;

    private void Awake()
    {
      _logging = RootContext.Resolve<IGameLog>();
      _timeService = RootContext.Resolve<ITimeService>();
    }

    private void OnTriggerEnter(Collider other)
    {
      _logging.Log("Triggered");
      _wasColldedWith = true;
    }

    private void OnTriggerExit(Collider other)
    {
      _logging.Log("Exit trigger");
      _wasColldedWith = false;
    }

    private void OnDrawGizmos()
    {
      if (!sphereCollider) return;

      Gizmos.color = GetColor();
      Gizmos.DrawSphere(GetPosition(), sphereCollider.radius);
    }

    private void OnRenderObject()
    {
      if (CurrentBuild.GetConfiguration() == BuildConfiguration.Development)
      {
        DrawDebugRuntime.DrawTempWireSphere(
          center: GetPosition(),
          radius: sphereCollider.radius,
          color: GetColor(),
          segments: 16,
          duration: _timeService.DeltaTime
          );
      }
    }

    private Vector3 GetPosition()
    {
      return transform.position + sphereCollider.center;
    }
    private Color GetColor()
    {
      return _wasColldedWith ? TriggeredColor : UntriggeredColor;
    }
  }
}
