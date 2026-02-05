// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Elements.Common.LoadingScreen.Interfaces;

using Code.Common.Extensions.Async;
using Code.Infrastructure.StateMachine;

namespace Code.Infrastructure
{
  public class GameMod
  {
    public GameStateMachine StateMachine { get; private set; }

    public GameMod(ICoroutineRunner runner, ILoadScreen loadScreen)
    {
      StateMachine =
        new GameStateMachine(runner, loadScreen);
    }
  }
}
