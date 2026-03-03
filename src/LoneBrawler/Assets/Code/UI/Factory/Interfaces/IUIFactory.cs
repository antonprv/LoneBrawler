// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Data.StaticData.Types.UI;
using Code.UI.Windows.Types;

using Cysharp.Threading.Tasks;

using UnityEngine.UI;

namespace Code.UI.Factory.Interfaces
{
  public interface IUIFactory
  {
    public HashSet<WindowTypeId> OpenWindows { get; }

    public void Cleanup();
    public UniTask CreateWindow(WindowTypeId typeId, Button openButton, ConstructorContext context = ConstructorContext.InCode);
    public void CreateUIRootAsync();
    public UniTask WarmUp();
    public UniTask CreateMainMenuAsync(Button openButton = null, ConstructorContext context = ConstructorContext.InCode);
  }
}
