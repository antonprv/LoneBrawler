// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Configs.BuildConfig;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IBuildConfigSubservice
  {
    BuildConfiguration Current { get; }
    public bool IsDevelopment();
  }
}
