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
    public GameObject _tooltipRoot;
    public TextMeshProUGUI _buffNameText;
    public TextMeshProUGUI _descriptionText;
    public TextMeshProUGUI _activationTypeText;
    public TextMeshProUGUI _durationText;
    public TextMeshProUGUI _costText;
    public RectTransform _tooltipRect;
    public Canvas _canvas;
    public Vector2 _offset = new(10, 10);

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

      _tooltipRoot.SetActive(true);

      _buffNameText.text = buffData.DisplayName;
      _descriptionText.text = buffData.Description;
      _activationTypeText.text = $"Type: {buffData.ActivationType}";

      if (buffData.ActivationType != BuffActivationType.Burst)
      {
        _durationText.gameObject.SetActive(true);
        _durationText.text = $"Duration: {buffData.Duration:F1}s";
      }
      else
      {
        _durationText.gameObject.SetActive(false);
      }

      _costText.text = $"Cost: {buffData.Cost} souls";

      UpdatePosition(position);
    }

    public void Hide()
    {
      _tooltipRoot.SetActive(false);
    }

    public bool IsVisible()
    {
      return _tooltipRoot.activeSelf;
    }

    public void UpdatePosition(Vector3 screenPosition)
    {
      if (!_tooltipRoot.activeSelf)
        return;

      Camera cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

      RectTransformUtility.ScreenPointToLocalPointInRectangle(
        _canvas.transform as RectTransform,
        screenPosition,
        cam,
        out Vector2 localPoint);

      _tooltipRect.anchoredPosition = localPoint + _offset;
    }
  }
}
