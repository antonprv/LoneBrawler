// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Elements.Inventory;

using UnityEngine;

namespace Code.UI.Services.TooltipService.Interfaces
{
  public interface ITooltipService { }

  public interface ITooltipReceiver : ITooltipService
  {
    void SetTooltip(ItemTooltipController tooltip);
  }

  public interface ITooltipProvider : ITooltipService
  {
    ItemTooltipController GetTooltip();
  }
}
