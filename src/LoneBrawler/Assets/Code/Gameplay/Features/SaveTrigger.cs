// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.DebugUtils;
using Code.Common.Extensions.Logging;
using Code.Configs;
using Code.Infrastructure.Services.SaveLoad;

using Reflex.Attributes;

using UnityEngine;

namespace Code.Gameplay.Features
{
  public class SaveTrigger : MonoBehaviour
  {
    public BoxCollider BoxCollider;

    private IGameLog _logging;
    private ISaveLoadService _saveLoadService;
    private bool _wasColldedWith = false;

    [Inject]
    private void Construct(IGameLog logging, ISaveLoadService saveLoadService)
    {
      _logging = logging;
      _saveLoadService = saveLoadService;
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

      Gizmos.color = new Color(0.0f, 0.0f, 0.5f, 0.8f);
      Gizmos.DrawCube(transform.position + BoxCollider.center, BoxCollider.size);
    }

    private void OnRenderObject()
    {
      if (CurrentBuild.GetConfiguration() == BuildConfiguration.Development)
      {
        if (_wasColldedWith) return;
        DrawDebugRuntime.DrawTempWireCube(
          transform.position + BoxCollider.center,
          BoxCollider.size,
          new Color(0.0f, 0.0f, 0.5f, 0.5f)
          );
      }
    }
  }
}
