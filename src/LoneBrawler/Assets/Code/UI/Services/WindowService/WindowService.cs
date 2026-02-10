// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types;
using Code.UI.Factory.Interfaces;
using Code.UI.Services.WindowService.Interfaces;
using Code.Utils.Extensions.ReflexExtensions;

namespace Code.UI.Services.WindowService
{
  public class WindowService : IWindowService
  {
    private IUIFactory _uiFactory;

    public WindowService()
    {
      _uiFactory = RootContext.Resolve<IUIFactory>();
    }

    public void Open(WindowTypeId typeId)
    {
      switch (typeId)
      {
        case WindowTypeId.None:
          break;
        case WindowTypeId.Shop:
          _uiFactory.CreateShop(typeId);
          break;
        default:
          break;
      }
    }
  }
}
