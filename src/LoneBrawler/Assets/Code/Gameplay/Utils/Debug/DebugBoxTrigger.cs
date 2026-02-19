// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.DebugUtils;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Utils.Debug
{
  [RequireComponent(typeof(BoxCollider))]
  public class DebugBoxTrigger : ZenjexBehaviour
  {
    public BoxCollider boxCollider;

    public bool drawInEditor = true;
    public bool drawInWorldSpace = false;

    public Color idleColor = Color.beige;
    public Color triggerColor = Color.rebeccaPurple;

    private bool _wasColldedWith;

    [Zenjex] private ITimeService _timeService;
    [Zenjex] private IBuildConfigSubservice _build;

    private void OnTriggerEnter(Collider other) => _wasColldedWith = true;

    private void OnTriggerExit(Collider other) => _wasColldedWith = false;

    private void OnDrawGizmos() => DrawShapeInEditor();

    private void OnRenderObject()
    {
      if (_build.IsDevelopment())
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
