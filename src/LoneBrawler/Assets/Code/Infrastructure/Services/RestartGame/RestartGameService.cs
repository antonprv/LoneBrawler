// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.BuffService.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.PlayerPrefs.Interfaces;
using Code.Infrastructure.Services.RestartGame.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;
using Code.UI.Elements.Common.LoadingScreen.Interfaces;
using Code.UI.Factory.Interfaces;

using Cysharp.Threading.Tasks;

using R3;

namespace Code.Infrastructure.Services.RestartGame
{
  public class RestartGameService : IRestartGameService, IDisposable
  {
    #region Dependencies

    private readonly IPersistentProgressService _progressService;
    private readonly IGameStateMachine _stateMachine;
    private readonly IPlayerPrefsService _playerPrefs;
    private readonly IPlayerDataSubervice _playerData;
    private readonly IGameFactory _gameFactory;
    private readonly IAssetLoader _assetLoader;
    private readonly IUIFactory _uiFactory;
    private readonly IBuffTrackerService _buffTracker;
    private readonly ILoadScreen _loadScreen;

    #endregion

    #region R3

    private readonly List<IRestartHandler> _restartHandlers = new();
    public IReadOnlyList<IRestartHandler> RestartHandlers => _restartHandlers;

    private readonly Subject<Unit> _onRestartRequested = new();
    public Observable<Unit> OnRestartRequested => _onRestartRequested;

    private readonly CompositeDisposable _disposables = new();

    #endregion

    #region Flags

    private bool _restartInProgress;

    #endregion

    #region UniTask

    private readonly CancellationTokenSource _cancellationToken = new();

    #endregion

    public RestartGameService(
      IPersistentProgressService progressService,
      IPlayerPrefsService playerPrefsService,
      IGameStateMachine stateMachine,
      IPlayerDataSubervice playerData,
      IGameFactory gameFactory,
      IAssetLoader assetLoader,
      IUIFactory uIFactory,
      IBuffTrackerService buffTrackerService,
      ILoadScreen loadScreen
      )
    {
      _progressService = progressService;

      _stateMachine = stateMachine;
      _playerPrefs = playerPrefsService;
      _playerData = playerData;

      _gameFactory = gameFactory;
      _assetLoader = assetLoader;
      _uiFactory = uIFactory;
      _buffTracker = buffTrackerService;

      _loadScreen = loadScreen;
    }

    public void RegisterHandler(IRestartHandler handler) =>
      _restartHandlers.Add(handler);

    public void UnregisterHandler(IRestartHandler handler) =>
      _restartHandlers.Remove(handler);

    public void RequestRestart()
    {
      if (_restartInProgress) return;
      _restartInProgress = true;

      _onRestartRequested.OnNext(Unit.Default);

      if (_restartHandlers.Count == 0)
      {
        LaunchRestart().Forget();
        return;
      }

      WaitForHandlersThenRestart().Forget();
    }

    private async UniTaskVoid WaitForHandlersThenRestart()
    {
      UniTask<Unit>[] tasks = _restartHandlers
          .Select( handler => {
            return handler
            .PrepareForRestart()
            .LastAsync(_cancellationToken.Token)
            .AsUniTask();
          })
          .ToArray();

      await UniTask.WhenAll(tasks);
      LaunchRestart().Forget();
    }

    private async UniTaskVoid LaunchRestart()
    {
      await UniTask.WaitForSeconds(_playerData.DeathDelay);
      RestartGame();
    }

    private void RestartGame()
    {
      _loadScreen.Show();

      Cleanup();
      ClearPrefs();

      _disposables.Dispose();

      ResetGame();
      _restartInProgress = false;
    }

    private void Cleanup()
    {
      _buffTracker.Cleanup();
      _assetLoader.Cleanup();
      _gameFactory.Cleanup();
      _uiFactory.Cleanup();
      
    }

    private void ClearPrefs()
    {
      _playerPrefs.DeleteKey(_progressService.ProgressKey);
      _playerPrefs.Save();
    }

    private void ResetGame() => _stateMachine.EnterState<BootStrapperState>();

    public void Dispose()
    {
      _cancellationToken.Cancel();
      _cancellationToken.Dispose();
      _disposables.Dispose();
    }
  }
}
