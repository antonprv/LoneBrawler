// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;
using System.Linq;

using Code.Data.StaticData;
using Code.Data.StaticData.Types;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class WindowDataSubservice : IWindowDataSubservice
  {
    private Dictionary<WindowTypeId, WindowConfig> _windows;

    public void LoadSelf() => _windows = Resources
        .Load<WindowStaticData>(StaticDataAddresses.WindowDataPath)
      .Configs
      .ToDictionary(x => x.windowId, x => x);

    public WindowConfig ForWindow(WindowTypeId typeId) =>
      _windows.TryGetValue(typeId, out WindowConfig windowConfig)
      ? windowConfig
      : null;
  }
}
