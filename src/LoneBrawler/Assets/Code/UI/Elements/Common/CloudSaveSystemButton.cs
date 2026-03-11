// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Common
{
  public class CloudSaveSystemButton : ZenjexBehaviour
  {
    [Zenjex] private readonly IBuildConfigSubservice _buildConfigSubservice;

    protected override void OnAwake()
    {
      base.OnAwake();

      if (_buildConfigSubservice.UseCloudSave == false)
        gameObject.SetActive(false);
    }
  }
}
