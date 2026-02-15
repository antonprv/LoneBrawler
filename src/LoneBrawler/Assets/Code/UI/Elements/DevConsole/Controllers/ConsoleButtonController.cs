// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.UI.Elements.DevConsole.Controllers
{
  public class ConsoleButtonController : MonoBehaviour
  {
    private void Awake()
    {
      if (!IsDevelopmentBuild())
        gameObject.SetActive(false);
    }

    private bool IsDevelopmentBuild()
    {
      IBuildConfigSubservice buildConfig =
        RootContext.Resolve<IStaticDataService>().BuildConfig;

      return buildConfig.IsDevelopment();
    }
  }
}
