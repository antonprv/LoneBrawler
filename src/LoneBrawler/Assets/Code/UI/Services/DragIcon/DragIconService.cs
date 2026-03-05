// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Elements.Inventory;
using Code.UI.Services.DragIcon.Interfaces;

namespace Code.UI.Services.DragIcon
{
  public class DragIconService : IDragIconService, IDragIconReceiver, IDragIconProvider
  {
    private DragIconView _dragIcon;

    public void SetDragIcon(DragIconView dragIcon) => _dragIcon = dragIcon;

    public DragIconView GetDragIcon() => _dragIcon;
  }
}
