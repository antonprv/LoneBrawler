// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.Time;

using UnityEngine.UI;

using YG;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Utils
{
  public class PauseButtonComponent : ZenjexBehaviour
  {
    public Button button;

    [Zenjex] private readonly ITimeService _timeService;

    private bool _toggle;

    protected override void OnAwake()
    {
      base.OnAwake();

      button.onClick.AddListener(ToggleTime);
    }

    private void ToggleTime()
    {
      _toggle = !_toggle;

      if (_toggle)
        _timeService.StopTime();
      else if (!_toggle)
        _timeService.StartTime();

      YG2.PauseGame(_toggle);
    }
  }
}
