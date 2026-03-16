// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Infrastructure.Services.Localisation.Names;
using Code.Infrastructure.Services.LocalisationService;

using TMPro;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Inventory
{
  public class ItemTooltipController : ZenjexBehaviour
  {
    [Zenjex] private readonly ILocalisationService _localisation;

    public GameObject tooltipRoot;
    public TextMeshProUGUI buffNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI activationTypeText;
    public TextMeshProUGUI durationText;
    public RectTransform tooltipRect;
    public Canvas canvas;
    public Vector2 offset = new(10, 10);
    private string _currentLanguage;

    public void Construct()
    {
      _currentLanguage = _localisation.GetCurrentLanguage();

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

      buffNameText.text = GetDisplayNameLocalised(buffData);
      descriptionText.text = GetDescriptionLocalised(buffData);

      activationTypeText.text = GetLocalisedType(buffData);

      if (buffData.ActivationType != BuffActivationType.Burst)
      {
        durationText.gameObject.SetActive(true);
        durationText.text = GetDurationLocalised(buffData);
      }
      else
        durationText.gameObject.SetActive(false);

      UpdatePosition(position);
    }

    public void Hide() => tooltipRoot.SetActive(false);

    public bool IsVisible() => tooltipRoot.activeSelf;

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

    #region Localisation

    private string GetDescriptionLocalised(BuffStaticData buffData)
    {
      if (_currentLanguage == LanguageNames.Russian)
        return buffData.DescriptionRU;
      else
        return buffData.DescriptionEN;
    }

    private string GetDisplayNameLocalised(BuffStaticData buffData)
    {
      if (_currentLanguage == LanguageNames.Russian)
        return buffData.DisplayNameRU;
      else
        return buffData.DisplayNameEN;
    }

    private string GetDurationLocalised(BuffStaticData buffData)
    {
      if (_currentLanguage == LanguageNames.Russian)
        return $"Длительность: {buffData.Duration:F1} секунд";
      else
        return $"Duration: {buffData.Duration:F1}s";
    }

    private string GetLocalisedType(BuffStaticData buffData)
    {
      if (_currentLanguage == LanguageNames.Russian)
        return $"Тип: {buffData.ActivationType.GetRussianName()}";
      else
        return $"Type: {buffData.ActivationType}";
    }

    #endregion

  }
}
