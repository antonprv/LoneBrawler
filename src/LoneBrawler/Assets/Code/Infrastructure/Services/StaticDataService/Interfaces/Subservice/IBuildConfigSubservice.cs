// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Configs.Types;

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IBuildConfigSubservice
  {
    DebugConfiguration Current { get; }
    TargetPlatform TargetPlatform { get; }
    bool UseCloudSave { get; }
    bool UseAddSdk { get; }

    public bool IsDevelopment();
    public UniTask LoadSelfAsync();
  }
}
