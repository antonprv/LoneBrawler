// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;

using UnityEngine.UI;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.MainMenuButtons
{
  public class LoginButton : ZenjexBehaviour
  {
    public Button button;

    [Zenjex] IGameLog _logger;

    protected override void OnAwake() =>
      button.onClick.AddListener(HandleLogin);

    private void HandleLogin() =>
      _logger.Log("Logging to the YG servers...");
  }
}
