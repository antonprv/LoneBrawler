// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Gameplay.Common.Visuals.UI;
using Code.Infrastructure.StateMachine.States;

using UnityEngine;

namespace Code.Infrastructure
{
  public class GameIntance : MonoBehaviour, ICoroutineRunner
  {
    public GameObject LoadingScreen;

    private GameMod _gameMod;

    private void Awake()
    {
      ILoadScreen _loadScreen = LoadingScreen.GetComponent<LoadingCurtain>();

      _gameMod = new GameMod(this, _loadScreen);
      _gameMod.StateMachine.EnterState<BootStrapperState>();

      DontDestroyOnLoad(this);
    }
  }
}
