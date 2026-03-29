// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.


// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Types;

using R3;

namespace Code.UI.Services.PlatformControls.Interfaces
{
  public interface IPlatformControls
  {
    ReadOnlyReactiveProperty<ControlScheme> ControlSchemeRP { get; }

    ControlScheme GetCachedScheme();
    void SetScheme(ControlScheme scheme);
  }
}
