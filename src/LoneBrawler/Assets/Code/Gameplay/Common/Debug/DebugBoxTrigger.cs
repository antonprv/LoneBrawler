// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.DebugUtils;
using Code.Common.Extensions.ReflexExtensions;
using Code.Configs;
using Code.Gameplay.Common.Time;

using UnityEngine;

namespace Code.Gameplay.Common.Debug
{
  [RequireComponent(typeof(BoxCollider))]
  public class DebugBoxTrigger : MonoBehaviour
  {
    public BoxCollider boxCollider;

    public bool drawInEditor = true;
    public bool drawInWorldSpace = false;

    public Color idleColor = Color.beige;
    public Color triggerColor = Color.rebeccaPurple;

    private bool _wasColldedWith;
    private ITimeService _timeService;

    private void Awake() => _timeService = RootContext.Resolve<ITimeService>();

    private void OnTriggerEnter(Collider other) => _wasColldedWith = true;

    private void OnTriggerExit(Collider other) => _wasColldedWith = false;

    private void OnDrawGizmos() => DrawShapeInEditor();

    private void OnRenderObject()
    {
      if (CurrentBuild.GetConfiguration() == BuildConfiguration.Development)
      {
        DrawDebugRuntime.DrawTempWireCube(
          center: GetPosition(),
          size: boxCollider.size,
          color: _wasColldedWith ? triggerColor : idleColor,
          duration: _timeService.DeltaAtOffset
          );
      }
    }

    private void DrawShapeInEditor()
    {
      if (!boxCollider || !drawInEditor) return;

      Gizmos.color = triggerColor;

      if (drawInWorldSpace)
        DrawInWorldSpace();
      else
        Gizmos.DrawCube(GetPosition(), boxCollider.size);
    }

    private void DrawInWorldSpace()
    {
      Matrix4x4 oldMatrix = Gizmos.matrix;

      Gizmos.matrix = Matrix4x4.TRS(
        boxCollider.transform.TransformPoint(boxCollider.center),
        boxCollider.transform.rotation,
        boxCollider.transform.lossyScale
      );

      Gizmos.DrawCube(Vector3.zero, boxCollider.size);

      Gizmos.matrix = oldMatrix;
    }

    private Vector3 GetPosition() => transform.position + boxCollider.center;
  }
}
