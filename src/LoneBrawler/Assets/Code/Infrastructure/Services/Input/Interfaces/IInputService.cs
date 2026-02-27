// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;

using UnityEngine;

namespace Code.Infrastructure.Services.Input.Interfaces
{
  public interface IInputService
  {
    public Vector2 Axis { get; }
    public PairData<int, bool> ActiveHotbar { get; }

    public bool IsAttackButtonUp();


    public bool GameInputEnabled { get; set; }

    public bool IsConsoleButtonPressed();
    public bool IsConsoleSubmitPressed();
    public float GetConsoleHistoryAxis();
  }
}
