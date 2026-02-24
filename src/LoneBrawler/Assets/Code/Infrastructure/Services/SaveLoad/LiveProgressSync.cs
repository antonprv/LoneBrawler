// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Installer.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;

using UnityEngine;

using Zenjex.Extensions.Core;

namespace Code.Infrastructure.Services.SaveLoad
{
  /// <summary>
  /// Keeps PlayerWorldData.TransformOnLevel up to date in memory while the player moves.
  ///
  /// This is not a save operation — it writes only to IPersistentProgressService.Progress
  /// (RAM), not to disk or Yandex. The actual save still happens via SaveLoadService
  /// on triggers, level transitions, and pagehide.
  ///
  /// Why this is needed:
  ///   IProgressWriter.WriteToProgress() is called only when SaveProgress() fires.
  ///   Without LiveProgressSync, pagehide saves the position from the last explicit
  ///   save — potentially minutes behind the real position.
  /// </summary>
  public sealed class LiveProgressSync : MonoBehaviour, IGameInstanceComponent
  {
    private const float SyncIntervalSeconds = 5f;

    private ISaveLoadService _saveLoad;
    private IGameLog _logger;

    public void DelayedAwake()
    {
      _logger = RootContext.Resolve<IGameLog>();
      _saveLoad = RootContext.Resolve<ISaveLoadService>();
      StartCoroutine(SyncLoop());
    }

    private void OnDisable() => StopAllCoroutines();

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

      // Важно: синхронный вызов
      _saveLoad?.SaveProgress();
    }
  }
}
