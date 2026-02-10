// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Utils.Extensions.ReflexExtensions;

using UnityEngine;

namespace Code.UI.Elements.DevConsole
{
  public class DevConsoleButton : MonoBehaviour
  {
    private IBuildConfigSubservice _buildConfig;

    private void Awake()
    {
      _buildConfig = RootContext.Resolve<IBuildConfigSubservice>();

      if (!_buildConfig.IsDevelopment())
        gameObject.SetActive(false);
    }
  }
}
