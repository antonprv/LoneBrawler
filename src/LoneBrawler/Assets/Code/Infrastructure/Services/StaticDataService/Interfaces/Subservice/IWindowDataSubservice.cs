// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Data.StaticData;
using Code.Data.StaticData.Types;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IWindowDataSubservice
  {
    Task LoadSelfAsync();
    Task<WindowStaticData> ForWindowAsync(WindowTypeId typeId);
  }
}
