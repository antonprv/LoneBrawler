// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;
using Code.Infrastructure.StateMachine.Types;

using UnityEngine.UI;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.MainMenuButtons
{
  public class ReturnToMenu : ZenjexBehaviour
  {
    public Button returnToMenuButton;

    [Zenjex] private readonly IGameStateMachine _stateMachine;

    protected override void OnAwake()
    {
      base.OnAwake();

      if (_stateMachine.GetCurrentStateType() == StateType.MainMenu)
        gameObject.SetActive(false);

      returnToMenuButton.onClick.AddListener(HandleReturnToMenu);
    }

    private void HandleReturnToMenu() => _stateMachine.EnterState<MainMenuState>();
  }
}
