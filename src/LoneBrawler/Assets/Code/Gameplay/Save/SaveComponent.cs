// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Gameplay.Save.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Save
{
  public class SaveComponent : MonoBehaviour, ISaveComponent
  {
    private IGameLog _logging;
    private ISaveLoadService _saveLoad;
    private IPersistentProgressService _progressService;

    public void Construct(IGameLog log, ISaveLoadService saveLoad, IPersistentProgressService progress)
    {
      _logging = log;
      _saveLoad = saveLoad;
      _progressService = progress;
    }

    public void Save()
    {
      _saveLoad.SaveProgress();
      _logging.Log("Saved game!");
      _progressService.Progress = _saveLoad.LoadProgress();
    }
  }
}
