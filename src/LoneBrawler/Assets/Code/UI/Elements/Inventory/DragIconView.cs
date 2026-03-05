// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Elements.Inventory
{
  public class DragIconView : MonoBehaviour
  {
    public Image icon;

    public RectTransform rectTransform;
    public Canvas parentCanvas;

    public void Construct() => Hide();

    public void Show(Sprite sprite, Vector2 screenPos)
    {
      icon.sprite = sprite;
      gameObject.SetActive(true);
      UpdatePosition(screenPos);
    }

    public void Hide() => gameObject.SetActive(false);

    public void UpdatePosition(Vector2 screenPos)
    {
      Camera cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
        ? null
        : parentCanvas.worldCamera;

      RectTransformUtility.ScreenPointToLocalPointInRectangle(
        parentCanvas.transform as RectTransform,
        screenPos,
        cam,
        out Vector2 local);

      rectTransform.anchoredPosition = local;
    }
  }
}
