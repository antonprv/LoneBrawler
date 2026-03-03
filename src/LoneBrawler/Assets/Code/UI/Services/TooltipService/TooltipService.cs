// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Elements.Inventory;
using Code.UI.Services.TooltipService.Interfaces;

namespace Code.UI.Services.TooltipService
{
  public class TooltipService : ITooltipService, ITooltipProvider, ITooltipReceiver
  {
    private ItemTooltipController _tooltip;

    public ItemTooltipController GetTooltip() => _tooltip;

    public void SetTooltip(ItemTooltipController tooltip) => _tooltip = tooltip;
  }
}
