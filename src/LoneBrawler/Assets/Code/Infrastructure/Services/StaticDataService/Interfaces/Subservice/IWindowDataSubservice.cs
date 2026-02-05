// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types;
using Code.UI.Types;

namespace Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice
{
  public interface IWindowDataSubservice
  {
    WindowConfig ForWindow(WindowTypeId typeId);
    void LoadSelf();
  }
}