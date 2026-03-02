// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IInventoryConfigSubservice
  {
    int HotbarSize { get; }
    int InventorySize { get; }

    UniTask LoadSelfAsync();
  }
}
