// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Unity.VisualScripting;

namespace Code.Infrastructure
{
  public class Game
  {
    public GameStateMachine StateMachine { get; private set; }
    public Game()
    {
     StateMachine = new GameStateMachine();
    }
  }
}
