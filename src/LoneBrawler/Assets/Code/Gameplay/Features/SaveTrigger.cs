// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Services.SaveLoad;

using UnityEngine;

namespace Code.Gameplay.Features
{
  public class SaveTrigger : MonoBehaviour
  {
    public BoxCollider BoxCollider;

    private IGameLog _logging;
    private ISaveLoadService _saveLoadService;

    private void Awake()
    {
      _logging = RootContext.Resolve<IGameLog>();
      _saveLoadService = RootContext.Resolve<ISaveLoadService>();
    }

    private void OnTriggerEnter(Collider other)
    {
      _saveLoadService.SaveProgress();
      _logging.Log("GameSaved");
      gameObject.SetActive(false);
    }

  }
}
