// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.DebugUtils;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Configs;
using Code.Gameplay.Common.Time;
using Code.Infrastructure.Services.SaveLoad;

using UnityEngine;

namespace Code.Gameplay.Features
{
  public class SaveTrigger : MonoBehaviour
  {
    public BoxCollider BoxCollider;

    public Color TriggerColor = new Color(0.0f, 0.0f, 0.5f, 0.5f);

    private IGameLog _logging;
    private ITimeService _timeService;
    private ISaveLoadService _saveLoadService;

    private bool _wasColldedWith = false;

    private void Awake()
    {
      _logging = RootContext.Resolve<IGameLog>();
      _timeService = RootContext.Resolve<ITimeService>();
      _saveLoadService = RootContext.Resolve<ISaveLoadService>();
    }

    private void OnTriggerEnter(Collider other)
    {
      _saveLoadService.SaveProgress();
      _logging.Log("GameSaved");
      gameObject.SetActive(false);
      _wasColldedWith = true;
    }

    private void OnDrawGizmos()
    {
      if (!BoxCollider) return;

      Gizmos.color = TriggerColor;
      Gizmos.DrawCube(GetPosition(), BoxCollider.size);
    }

    private void OnRenderObject()
    {
      if (CurrentBuild.GetConfiguration() == BuildConfiguration.Development)
      {
        if (_wasColldedWith) return;
        DrawDebugRuntime.DrawTempWireCube(
          center: GetPosition(),
          size: BoxCollider.size,
          color: TriggerColor,
          duration: _timeService.DeltaAtOffset
          );
      }
    }

    private Vector3 GetPosition()
    {
      return transform.position + BoxCollider.center;
    }
  }
}
