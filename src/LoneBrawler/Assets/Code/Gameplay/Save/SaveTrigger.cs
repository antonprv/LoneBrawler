// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;

using UnityEngine;

namespace Code.Gameplay.Save
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
