// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Gameplay.Services.Input;

using UnityEngine;

namespace Code.Infrastructure.Services.Input
{
  public class PCInputService : IInputService, IDisposable
  {
    private const string _horizontal = "Horizontal";
    private const string _vertical = "Vertical";
    private const string _fire = "Fire";

    private PlatformInputs _platformInputs;

    public PCInputService()
    {
      _platformInputs = new PlatformInputs();
      _platformInputs.Enable();
      _platformInputs.PlayerMap.Enable();
    }

    public Vector2 Axis =>
      GetSimpleInputAxes() == Vector2.zero ? GetPCInputAxes() : GetSimpleInputAxes();

    public bool IsAttackButtonUp() =>
      !GetSimpleAttackButtonUp() ? GetPCAttackButtonUp() : GetSimpleAttackButtonUp();

    Vector2 GetPCInputAxes() =>
      _platformInputs.PlayerMap.Move.ReadValue<Vector2>();
    Vector2 GetSimpleInputAxes() =>
      new(SimpleInput.GetAxis(_horizontal), SimpleInput.GetAxisRaw(_vertical));

    bool GetPCAttackButtonUp() => _platformInputs.PlayerMap.Fire.WasReleasedThisFrame();
    bool GetSimpleAttackButtonUp() => SimpleInput.GetButtonUp(_fire);

    public void Dispose()
    {
      _platformInputs.PlayerMap.Disable();
      _platformInputs.Disable();
      _platformInputs.Dispose();
    }
  }
}
