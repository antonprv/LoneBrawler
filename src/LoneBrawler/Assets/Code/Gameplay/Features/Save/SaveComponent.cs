// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Gameplay.Features.Save.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Features.Save
{
  public class SaveComponent : MonoBehaviour, ISaveComponent
  {
    private IGameLog _logging;
    private ISaveLoadService _saveLoad;

    public void Construct(IGameLog log, ISaveLoadService saveLoad)
    {
      _logging = log;
      _saveLoad = saveLoad;
    }

    public void Save()
    {
      _saveLoad.SaveProgress();
      _logging.Log("Saved game!");
    }
  }
}
