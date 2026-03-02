// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.UI;

using Code.UI.Factory.Interfaces;
using Code.UI.Services.WindowService.Interfaces;
using Code.UI.Windows.Types;

namespace Code.UI.Services.WindowService
{
  public class WindowService : IWindowService
  {
    private readonly IUIFactory _uiFactory;

    public WindowService(IUIFactory uIFactory) => _uiFactory = uIFactory;

    public async void Open(WindowTypeId typeId)
    {
      switch (typeId)
      {
        case WindowTypeId.None:
          break;
        case WindowTypeId.Shop:
          await _uiFactory.CreateWindow(typeId, ConstructorContext.FromButton);
          break;
        case WindowTypeId.MainMenu:
          await _uiFactory.CreateMainMenuAsync(ConstructorContext.FromButton);
          break;
        case WindowTypeId.Inventory:
          await _uiFactory.CreateWindow(typeId, ConstructorContext.FromButton);
          break;
        default:
          break;
      }
    }
  }
}
