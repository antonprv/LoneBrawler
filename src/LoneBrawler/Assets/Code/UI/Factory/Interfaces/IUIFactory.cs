// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Data.StaticData.Types.UI;
using Code.UI.Windows.Types;

namespace Code.UI.Factory.Interfaces
{
  public interface IUIFactory
  {
    public void Cleanup();
    public Task CreateWindow(WindowTypeId typeId, ConstructorContext context = ConstructorContext.InCode);
    public void CreateUIRootAsync();
    public Task WarmUp();
    public Task CreateMainMenuAsync(ConstructorContext context = ConstructorContext.InCode);
  }
}
