// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Gameplay.Services.Input
{
  public class PhoneInputService : IInputService
  {
    private const string _horizontal = "Horizontal";
    private const string _vertical = "Vertical";
    private const string _fire = "Fire";

    public Vector2 Axis =>
      new Vector2(SimpleInput.GetAxis(_horizontal), SimpleInput.GetAxis(_vertical));

    public bool IsAttackButtonUp() => SimpleInput.GetButtonUp(_fire);
  }
}
