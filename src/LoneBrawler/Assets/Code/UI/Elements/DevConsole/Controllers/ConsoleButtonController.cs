// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.DevConsole.Controllers
{
  public class ConsoleButtonController : ZenjexBehaviour
  {
    [Zenjex] private readonly IBuildConfigSubservice _buildConfig;

    protected override void OnAwake()
    {
      if (!IsDevelopmentBuild())
        gameObject.SetActive(false);
    }

    private bool IsDevelopmentBuild() => _buildConfig.IsDevelopment();
  }
}
