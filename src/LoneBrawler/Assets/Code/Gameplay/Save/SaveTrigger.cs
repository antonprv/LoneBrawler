// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Services.SaveLoad.Interfaces;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Save
{
  public class SaveTrigger : ZenjexBehaviour
  {
    public BoxCollider BoxCollider;

    [Zenjex] private IGameLog _logging;
    [Zenjex] private ISaveLoadService _saveLoadService;

    private void OnTriggerEnter(Collider other)
    {
      _saveLoadService.SaveProgress();
      _logging.Log("GameSaved");
      gameObject.SetActive(false);
    }

  }
}
