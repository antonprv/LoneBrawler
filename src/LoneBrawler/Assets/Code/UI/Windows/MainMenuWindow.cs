// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.UI.Windows.Types;

using UnityEngine.UI;

using Zenjex.Extensions.Core;

namespace Code.UI.Windows
{
  public class MainMenuWindow : WindowBase
  {
    public Button loadSave;
    public override WindowTypeId WindowType => WindowTypeId.MainMenu;

    private IGameLog _logger;
    private ISaveLoadService _saveLoad;

    public override void Construct(
      IPersistentProgressService progressService,
      ConstructorContext context,
      Button openButton
      ) =>
      base.Construct(progressService, context, openButton);

    protected override void InjectDependencies()
    {
      base.InjectDependencies();

      _logger = RootContext.Resolve<IGameLog>();
      _saveLoad = RootContext.Resolve<ISaveLoadService>();
    }

    protected override void Initialize()
    {
      CheckContext();
      CheckPlayerProgress();
    }

    private void CheckContext()
    {
      if (ConstructorContext != ConstructorContext.FromButton)
        closeWindow.gameObject.SetActive(false);
      else
        closeWindow.gameObject.SetActive(true);
    }

    protected override void SubscribeUpdates() => CheckPlayerProgress();

    protected override void Cleanup() => base.Cleanup();

    private void CheckPlayerProgress()
    {
      _logger.Log("Checking player progress...");

      var loadedProgress = _saveLoad.LoadProgress();

      if (loadedProgress.SaveTimeUTC == 0)
      {
        _logger.Log("No valid save found - hiding load save button");
        loadSave.gameObject.SetActive(false);
      }
      else
      {
        _logger.Log($"Save found! SaveTimeUTC: {loadedProgress.SaveTimeUTC} - showing load save button");
        loadSave.gameObject.SetActive(true);
      }
    }
  }
}
