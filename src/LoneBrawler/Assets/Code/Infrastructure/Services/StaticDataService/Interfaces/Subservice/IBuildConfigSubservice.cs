// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Data.StaticData.Configs.Types;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IBuildConfigSubservice
  {
    BuildConfiguration Current { get; }
    TargetPlatform TargetPlatform { get; }
    public bool IsDevelopment();
    public Task LoadSelfAsync();
  }
}
