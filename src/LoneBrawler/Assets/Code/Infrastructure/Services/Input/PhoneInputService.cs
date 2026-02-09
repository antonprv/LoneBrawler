// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.Input.Interfaces;

using UnityEngine;

namespace Code.Infrastructure.Services.Input
{
  public class PhoneInputService : IInputService
  {
    public bool GameInputEnabled { get; set; }

    public Vector2 Axis =>
      new Vector2(
        SimpleInput.GetAxis(TouchButtonNames.HorizontalAxis),
        SimpleInput.GetAxisRaw(TouchButtonNames.VerticalAxis)
        );

    public bool IsAttackButtonUp() =>
      SimpleInput.GetButtonUp(TouchButtonNames.AttackButton);

    // Console input - mobile uses visual button and touch keyboard instead
    public bool IsConsoleButtonPressed() =>
      SimpleInput.GetButtonUp(TouchButtonNames.ToggleConsoleButton);
    public bool IsConsoleSubmitPressed() =>
      SimpleInput.GetButtonUp(TouchButtonNames.ConsoleSubmitButton);
    public float GetConsoleHistoryAxis() => 0f;
  }
}
