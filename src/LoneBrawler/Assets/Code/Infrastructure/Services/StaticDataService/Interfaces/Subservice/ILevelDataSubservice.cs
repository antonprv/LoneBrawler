// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Data.StaticData;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface ILevelDataSubservice
  {
    public Task LoadSelfAsync();
    public Task<LevelStaticData> ForLevelAsync(string sceneKey);
  }
}
