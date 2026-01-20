// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Async;
using Code.Gameplay.Common.Visuals.UI;
using Code.Infrastructure.StateMachine;

namespace Code.Infrastructure
{
  public class GameMod
  {
    public GameStateMachine StateMachine { get; private set; }
    public GameMod(ICoroutineRunner runner, ILoadScreen curtain)
    {
      StateMachine = new GameStateMachine(runner, curtain);
    }
  }
}
