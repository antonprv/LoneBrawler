// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.UI;

using Code.UI.Factory.Interfaces;
using Code.UI.Services.WindowService.Interfaces;
using Code.UI.Windows.Types;

using Cysharp.Threading.Tasks;

using UnityEngine.UI;

namespace Code.UI.Services.WindowService
{
  public class WindowService : IWindowService
  {
    private readonly IUIFactory _uiFactory;

    public WindowService(IUIFactory uIFactory) => _uiFactory = uIFactory;

    public void Open(WindowTypeId typeId, Button openButton)
    {
      switch (typeId)
      {
        case WindowTypeId.None:
          break;
        case WindowTypeId.Shop:
          _uiFactory
            .CreateWindow(typeId, openButton, ConstructorContext.FromButton)
            .Forget();
          break;
        case WindowTypeId.MainMenu:
          _uiFactory
            .CreateMainMenuAsync(openButton, ConstructorContext.FromButton)
            .Forget();
          break;
        case WindowTypeId.Inventory:
          _uiFactory
            .CreateWindow(typeId, openButton, ConstructorContext.FromButton)
            .Forget();
          break;
        default:
          break;
      }
    }
  }
}
