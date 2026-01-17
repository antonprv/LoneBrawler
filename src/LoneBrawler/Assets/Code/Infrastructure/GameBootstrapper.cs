// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using TMPro.EditorUtilities;

using UnityEngine;

namespace Code.Infrastructure
{
  public class GameBootstrapper : MonoBehaviour
  {
    private Game _game;

    private void Awake()
    {
      _game = new Game();
      _game.StateMachine.EnterState<BootStrapperState>();
    }
  }
}
