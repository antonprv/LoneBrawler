// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Generated.Input;
using Code.Infrastructure.Services.Input.Interfaces;

using UnityEngine;

namespace Code.Infrastructure.Services.Input
{
  public class PCInputService : IInputService, IDisposable
  {
    public bool GameInputEnabled { get; set; }

    private readonly PlatformInputs _platformInputs;

    public PCInputService()
    {
      GameInputEnabled = true;

      _platformInputs = new PlatformInputs();
      _platformInputs.Enable();
      _platformInputs.PlayerMap.Enable();
    }

    public Vector2 Axis
    {
      get
      {
        if (!GameInputEnabled) return Vector2.zero;
        return GetSimpleInputAxes() == Vector2.zero ? GetPCInputAxes() : GetSimpleInputAxes();
      }
    }

    public bool IsAttackButtonUp()
    {
      if (!GameInputEnabled) return false;
      return !GetTouchAttackButtonUp() ? GetPCAttackButtonUp() : GetTouchAttackButtonUp();
    }

    // Console input methods
    public bool IsConsoleButtonPressed() => !GetTouchConsoleButtonUp() ?
      GetPCConsoleButtonUp() : GetTouchConsoleButtonUp();

    public bool IsConsoleSubmitPressed() => !GetTouchConsoleSubmitButtonUp() ?
      GetPCConsoleSubmitButtonUp() : GetTouchConsoleSubmitButtonUp();

    public float GetConsoleHistoryAxis()
    {
      float navigationAxis = _platformInputs.PlayerMap.ConsoleNavigation.ReadValue<float>();
      return navigationAxis;
    }

    #region private methods
    private Vector2 GetPCInputAxes() =>
      _platformInputs.PlayerMap.Move.ReadValue<Vector2>();
    private Vector2 GetSimpleInputAxes() =>
      new(
        SimpleInput.GetAxis(TouchButtonNames.HorizontalAxis),
        SimpleInput.GetAxisRaw(TouchButtonNames.VerticalAxis)
        );

    private bool GetPCAttackButtonUp() =>
      _platformInputs.PlayerMap.Attack.WasReleasedThisFrame();
    private bool GetPCConsoleButtonUp() =>
      _platformInputs.PlayerMap.Console.WasPressedThisFrame();
    private bool GetPCConsoleSubmitButtonUp() =>
      _platformInputs.PlayerMap.ConsoleSubmit.WasPressedThisFrame();

    private bool GetTouchAttackButtonUp() =>
      SimpleInput.GetButtonUp(TouchButtonNames.AttackButton);
    private bool GetTouchConsoleButtonUp() =>
      SimpleInput.GetButtonUp(TouchButtonNames.ToggleConsoleButton);
    private bool GetTouchConsoleSubmitButtonUp() =>
      SimpleInput.GetButtonUp(TouchButtonNames.ConsoleSubmitButton);

    public void Dispose()
    {
      _platformInputs.PlayerMap.Disable();
      _platformInputs.Disable();
      _platformInputs.Dispose();
    }
  }
  #endregion
}
