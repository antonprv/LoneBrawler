// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Services.SaveLoad.Interfaces;

namespace Code.Gameplay.Features.Save.Interfaces
{
  public interface ISaveComponent
  {
    void Construct(IGameLog log, ISaveLoadService saveLoad);
    void Save();
  }
}
