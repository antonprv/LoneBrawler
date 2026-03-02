// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Utils.ActorComponents;

using TMPro;

using UnityEngine;

namespace Code.UI.Elements.Inventory
{
  public class ItemTooltipController : AsyncStartMonoBehaviour
  {
    public GameObject tooltipRoot;
    public TextMeshProUGUI buffNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI activationTypeText;
    public TextMeshProUGUI durationText;
    public RectTransform tooltipRect;
    public Canvas canvas;
    public Vector2 offset = new(10, 10);

    protected override void AsyncStart()
    {
      base.AsyncStart();
      Hide();
    }

    public void Show(BuffStaticData buffData, Vector3 position)
    {
      if (buffData == null)
      {
        Hide();
        return;
      }

      tooltipRoot.SetActive(true);

      buffNameText.text = buffData.DisplayName;
      descriptionText.text = buffData.Description;
      activationTypeText.text = $"Type: {buffData.ActivationType}";

      if (buffData.ActivationType != BuffActivationType.Burst)
      {
        durationText.gameObject.SetActive(true);
        durationText.text = $"Duration: {buffData.Duration:F1}s";
      }
      else
      {
        durationText.gameObject.SetActive(false);
      }

      UpdatePosition(position);
    }

    public void Hide()
    {
      tooltipRoot.SetActive(false);
    }

    public bool IsVisible()
    {
      return tooltipRoot.activeSelf;
    }

    public void UpdatePosition(Vector3 screenPosition)
    {
      if (!tooltipRoot.activeSelf)
        return;

      Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

      RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform,
        screenPosition,
        cam,
        out Vector2 localPoint);

      tooltipRect.anchoredPosition = localPoint + offset;
    }
  }
}
