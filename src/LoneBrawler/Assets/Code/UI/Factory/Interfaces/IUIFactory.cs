// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Types;

namespace Code.UI.Factory.Interfaces
{
  public interface IUIFactory
  {
    void CreateShop(WindowTypeId typeId);
    void CreateUIRoot();
  }
}
