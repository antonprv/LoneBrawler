// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.DebugUtils;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using UnityEngine;

namespace Code.Gameplay.Common.Debug
{
  [RequireComponent(typeof(SphereCollider))]
  public class DebugSphereTrigger : MonoBehaviour
  {
    public SphereCollider sphereCollider;

    public bool drawInEditor = true;
    public bool drawInWorldSpace = false;

    public Color UntriggeredColor = Color.beige;
    public Color TriggeredColor = Color.rebeccaPurple;

    private IGameLog _logging;
    private ITimeService _timeService;
    private IStaticDataService _staticDataService;
    private IBuildConfigSubservice _build;
    private bool _wasColldedWith = false;

    private void Awake()
    {
      _logging = RootContext.Resolve<IGameLog>();
      _timeService = RootContext.Resolve<ITimeService>();
      _staticDataService = RootContext.Resolve<IStaticDataService>();

      _build = _staticDataService.BuildConfig;
    }

    private void OnTriggerEnter(Collider other) => _wasColldedWith = true;

    private void OnTriggerExit(Collider other) => _wasColldedWith = false;

    private void OnDrawGizmos() => DrawShapeInEditor();

    private void OnRenderObject()
    {
      if (_build.IsDevelopment())
      {
        DrawDebugRuntime.DrawTempWireSphere(
          center: GetPosition(),
          radius: sphereCollider.radius,
          color: GetColor(),
          segments: 12,
          duration: _timeService.DeltaAtOffset
          );
      }
    }

    private void DrawShapeInEditor()
    {
      if (!sphereCollider || !drawInEditor) return;

      Gizmos.color = GetColor();

      if (drawInWorldSpace)
        DrawInWorldSpace();
      else
        Gizmos.DrawSphere(GetPosition(), sphereCollider.radius);
    }

    private void DrawInWorldSpace()
    {
      Matrix4x4 oldMatrix = Gizmos.matrix;

      Transform t = sphereCollider.transform;

      float maxScale = Mathf.Max(
        t.lossyScale.x,
        t.lossyScale.y,
        t.lossyScale.z
      );

      Gizmos.matrix = Matrix4x4.TRS(
        t.TransformPoint(sphereCollider.center),
        t.rotation,
        Vector3.one
      );

      Gizmos.DrawSphere(Vector3.zero, sphereCollider.radius * maxScale);

      Gizmos.matrix = oldMatrix;
    }

    private Vector3 GetPosition() => transform.position + sphereCollider.center;
    private Color GetColor() => _wasColldedWith ? TriggeredColor : UntriggeredColor;
  }
}
