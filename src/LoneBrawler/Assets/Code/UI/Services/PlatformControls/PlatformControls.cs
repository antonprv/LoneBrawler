// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Services.PlatformControls.Interfaces;

using Code.Data.SaveData.Types;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;

using R3;

namespace Code.UI.Services.PlatformControls
{
  public class PlatformControls : IPlatformControls
  {
    private ReactiveProperty<ControlScheme> _controlSchemeRP =
      new(ControlScheme.None);

    public ReadOnlyReactiveProperty<ControlScheme> ControlSchemeRP => _controlSchemeRP;

    private readonly ISaveLoadService _saveLoad;
    private readonly IPersistentProgressService _progress;

    public PlatformControls(
      ISaveLoadService saveLoadService,
      IPersistentProgressService progressService
      )
    {
      _saveLoad = saveLoadService;
      _progress = progressService;
    }

    public void SetScheme(ControlScheme scheme)
    {
      if (_controlSchemeRP.CurrentValue == scheme) return;

      _controlSchemeRP.Value = scheme;

      _progress
        .SystemSettings
        .Controls = _controlSchemeRP.CurrentValue;

      _saveLoad.SaveProgress();
    }

    public ControlScheme GetCachedScheme() => _progress.SystemSettings.Controls;
  }
}
