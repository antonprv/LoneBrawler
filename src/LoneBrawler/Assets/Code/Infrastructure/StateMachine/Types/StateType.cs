// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.StateMachine.Types
{
  [System.Serializable]
  public enum StateType
  {
    None = 0,
    BootStrapper = 1,
    LoadProgress = 2,
    MainMenu = 3,
    LoadLevel = 4,
    GameLoop = 5
  }
}
