// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.UI.Services.Factory.Interfaces;

using Code.UI.Services.WindowService.Interfaces;
using Code.UI.Types;

namespace Code.UI.Services.WindowService
{
  public class WindowService : IWindowService
  {
    private IUIFactory _uiFactory;

    public WindowService()
    {
      _uiFactory = RootContext.Resolve<IUIFactory>();
    }

    public void Open(WindowId windowId)
    {
      switch (windowId)
      {
        case WindowId.None:
          break;
        case WindowId.Shop:
          _uiFactory.CreateShop();
          break;
        default:
          break;
      }
    }
  }
}
