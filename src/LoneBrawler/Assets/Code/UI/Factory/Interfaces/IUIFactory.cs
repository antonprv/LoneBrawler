// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Data.StaticData.Types;

namespace Code.UI.Factory.Interfaces
{
  public interface IUIFactory
  {
    public void Cleanup();
    public void CreateShop(WindowTypeId typeId);
    public void CreateUIRootAsync();
    public Task WarmUp();
  }
}
