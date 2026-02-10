// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types;

using Code.UI.Services.WindowService.Interfaces;

using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Elements
{
  public class OpenWindowButton : MonoBehaviour
  {
    public Button button;
    public WindowTypeId windowType;

    private IWindowService _windowService;

    public void Construct(IWindowService windowService)
    {
      _windowService = windowService;
      button.onClick.AddListener(Open);
    }

    private void Open()
    {
      _windowService.Open(windowType);
    }
  }
}
