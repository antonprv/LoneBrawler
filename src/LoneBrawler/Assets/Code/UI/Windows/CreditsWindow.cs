// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.UI;

namespace Code.UI.Windows
{
  class CreditsWindow : WindowBase
  {
    protected override void SetWindowType() =>
      windowTypeId = WindowTypeId.Credits;
  }
}
