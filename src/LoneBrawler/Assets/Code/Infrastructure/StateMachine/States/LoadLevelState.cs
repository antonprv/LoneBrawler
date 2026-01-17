// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Assets.Code.Gameplay.Services.SceneLoader;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Visuals.UI;
using Code.Gameplay.Features.GameplayCamera;

using UnityEngine;

namespace Code.Infrastructure.StateMachine.States
{
  internal class LoadLevelState : IGamePayloadedState<string>
  {
    private const string PlayerStartTag = "PlayerStart";

    private GameStateMachine _gameStateMachine;
    private ICoroutineRunner _runner;
    private ILoadScreen _curtain;
    private IGameLog _logger;
    private Camera _camera;
    private readonly ISceneLoader _sceneLoader;

    public LoadLevelState(
      GameStateMachine gameStateMachine,
      ICoroutineRunner runner,
      ILoadScreen curtain)
    {
      _logger = RootContext.Resolve<IGameLog>();
      _sceneLoader = RootContext.Resolve<ISceneLoader>();

      _gameStateMachine = gameStateMachine;
      _runner = runner;

      _curtain = curtain;
    }

    public void Enter(string payload)
    {
      _curtain.Show();
      _logger.Log("Entered state");
      _sceneLoader.Load(payload, _runner, onSceneLoaded: OnLevelLoaded);
    }
    public void Exit()
    {
      _curtain.Hide();
      _logger.Log("Exited state");
    }

    private void OnLevelLoaded()
    {
      GameObject hero = Instantiate("Hero/hero");
      Instantiate("Hud/Hud");

      _camera = Camera.main;
      CameraFollow(hero);

      PlaceHero(hero);

      _gameStateMachine.EnterState<GameLoopState>();
    }

    private static void PlaceHero(GameObject hero)
    {
      var playerStart = GameObject.FindWithTag(PlayerStartTag);
      hero.transform.position = playerStart.transform.position;
      hero.transform.rotation = playerStart.transform.rotation;
    }

    private static GameObject Instantiate(string path)
    {
      var prefab = (GameObject)Resources.Load(path);
      return Object.Instantiate(prefab);
    }

    private void CameraFollow(GameObject hero) => _camera.GetComponent<CameraFollow>().Follow(hero);

  }
}
