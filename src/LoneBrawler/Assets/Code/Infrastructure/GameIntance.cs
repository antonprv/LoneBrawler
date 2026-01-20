// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Visuals.UI;
using Code.Gameplay.Features.GameplayCamera;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.SceneLoader;
using Code.Infrastructure.StateMachine.States;

using UnityEngine;

namespace Code.Infrastructure
{
  public class GameIntance : MonoBehaviour, ICoroutineRunner
  {
    public GameObject LoadingScreen;

    private GameMod _gameMod;
    private GameStateDependencies _stateDependencies;

    private void Awake()
    {
      ResolveDependencies();
      ILoadScreen _loadScreen = LoadingScreen.GetComponent<LoadingCurtain>();

      _gameMod = new GameMod(this, _loadScreen, _stateDependencies);
      _gameMod.StateMachine.EnterState<BootStrapperState>();

      DontDestroyOnLoad(this);
    }

    void ResolveDependencies()
    {
      _stateDependencies.sceneLoader = RootContext.Resolve<ISceneLoader>();
      _stateDependencies.gameFactory = RootContext.Resolve<IGameFactory>();
      _stateDependencies.cameraManager = RootContext.Resolve<ICameraManager>();
    }
  }
}
