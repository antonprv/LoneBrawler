// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Data.StaticData.Types;

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IWindowDataSubservice
  {
    UniTask LoadSelfAsync();
    UniTask<WindowStaticData> ForWindowAsync(WindowTypeId typeId);
  }
}
