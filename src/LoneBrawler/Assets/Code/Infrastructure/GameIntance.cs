// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Elements.Common.LoadingScreen;
using Code.UI.Elements.Common.LoadingScreen.Interfaces;

using Code.Common.Extensions.Async;
using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
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
      IGameLog gameLog = RootContext.Resolve<IGameLog>();

      ILoadScreen _loadScreen = LoadingScreen.GetComponent<LoadingCurtain>();

      _gameMod = new GameMod(this, _loadScreen);
      _gameMod.StateMachine.EnterState<BootStrapperState>();

      DontDestroyOnLoad(this);
    }
  }
}
