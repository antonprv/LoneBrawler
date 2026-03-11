// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.Extensions.Logging;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.Services.SaveLoad.Interfaces;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Infrastructure.Services.SaveLoad
{
  public sealed class LiveProgressSync : ZenjexBehaviour, ILiveProgressSync
  {
    public float SyncIntervalSeconds => 5f;

    [Zenjex] private readonly ISaveLoadService _saveLoad;
    [Zenjex] private readonly IGameLog _logger;

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
