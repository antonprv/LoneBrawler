// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Configs;

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IMusicConfigSubservice
  {
    MusicPlayerConfig Confg { get; }

    UniTask LoadSelfAsync();
  }
}
