// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Installer.Interfaces;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.SaveLoad.Interfaces;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenjex.Extensions.Core;

namespace Code.Infrastructure.Services.SaveLoad
{
  public sealed class LiveProgressSync : MonoBehaviour, IGameInstanceComponent, ILiveProgressSync
  {
    public float SyncIntervalSeconds => 5f;

    private ISaveLoadService _saveLoad;
    private IGameLog _logger;

    public void DelayedAwake()
    {
      _saveLoad = RootContext.Resolve<ISaveLoadService>();
      _logger = RootContext.Resolve<IGameLog>();

      RootContext.Runtime.Bind<ILiveProgressSync>().FromInstance(this).AsSingle();
    }

    private void OnDisable() => StopAllCoroutines();

    public void StartSyncLoop()
    {
      if (SceneManager.GetActiveScene().name == SceneAddresses.MainMenuAddress)
        return;

      StartCoroutine(SyncLoop());
    }

    public void StopSyncLoop() => StopAllCoroutines();

    private IEnumerator SyncLoop()
    {
      var interval = new WaitForSeconds(SyncIntervalSeconds);

      while (true)
      {
        yield return interval;
        _logger.Log("Making live save...");
        _saveLoad.SaveProgress();
      }
    }

    /// <summary>
    /// Called by YG plugin when page is closing or refreshing.
    /// Must be public, without parameters and without overloads.
    /// </summary>
    public void OnQuitGame()
    {
      _logger?.Log("YG QuitGame event received. Forcing final save...");

      // Important: synchronous call
      _saveLoad?.SaveProgress();
    }
  }
}
