// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.UI;

using UnityEngine.UI;

namespace Code.UI.Services.WindowService.Interfaces
{
  public interface IWindowService
  {
    public void Open(WindowTypeId typeId, Button openButton);
  }
}
