// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Elements.Inventory;

namespace Code.UI.Services.DragIcon.Interfaces
{
  public interface IDragIconService { }

  public interface IDragIconReceiver : IDragIconService
  {
    void SetDragIcon(DragIconView dragIcon);
  }

  public interface IDragIconProvider : IDragIconService
  {
    DragIconView GetDragIcon();
  }
}
