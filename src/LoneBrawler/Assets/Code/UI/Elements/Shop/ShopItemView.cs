// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.UI.Services.InventoryService.Interfaces;

using Code.Common.Extensions.Logging;
using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Features.Player.Buffs.Interfaces;
using Code.Infrastructure.Services.SoulsTracker.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Zenjex.Extensions.Core;
using Zenjex.Extensions.Injector;
using Zenjex.Extensions.Attribute;
using Code.Infrastructure.Services.LocalisationService;
using Code.Infrastructure.Services.Localisation.Names;

namespace Code.UI.Elements.Shop
{
  public class ShopItemView : ZenjexBehaviour
  {
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI nameText;
    public Button purchaseButton;

    private int _amountInBundle;

    private BuffClassName _buffClass;
    private int _price;

    [Zenjex] private readonly IGameLog _logger;
    [Zenjex] private readonly IBuffDataSubservice _buffData;
    [Zenjex] private readonly IInventoryService _inventoryService;
    [Zenjex] private readonly ISoulsTrackerService _soulsTrackerService;
    [Zenjex] private readonly ILocalisationService _localisation;

    public void Construct(BuffClassName buffClass)
    {
      _buffClass = buffClass;
      SetupButton();
      LoadDataAsync().Forget();
      SetupAmount();
    }

    private void SetupAmount() =>
      amountText.text = _amountInBundle > 1 ?
      _amountInBundle.ToString() : string.Empty;

    public void SetIcon(Sprite icon)
    {
      if (iconImage == null || icon == null) return;
      iconImage.sprite = icon;
    }

    private void SetupButton() =>
      purchaseButton.onClick.AddListener(OnPurchaseClicked);

    private async UniTaskVoid LoadDataAsync()
    {
      BuffStaticData buffData = await _buffData.ForBuffAsync(_buffClass);

      if (buffData == null)
      {
        _logger.Log(LogType.Error, $"Couldn't load buff data: {_buffClass}");
        return;
      }

      if (priceText == null) return;

      _price = buffData.Cost;
      _amountInBundle = buffData.AmountInShop;

      priceText.text = _price.ToString();
      nameText.text = SetDisplayNameLocalised(buffData);
    }

    private string SetDisplayNameLocalised(BuffStaticData buffData)
    {
      var currentLang = _localisation.GetCurrentLanguage();

      if (currentLang == LanguageNames.Russian)
        return buffData.DisplayNameRU;
      else
        return buffData.DisplayNameEN;
    }

    private void OnPurchaseClicked()
    {
      if (_soulsTrackerService.TrySpendSouls(_price))
      {
        _inventoryService.AddBuffAsync(
          buffClass: _buffClass,
          count: _amountInBundle,
          tryHotbarFirst: true
          );

        var receiver = RootContext.Resolve<IBuffReceiver>();
        if (receiver == null) return;
        receiver.ReceiveBuff(_buffClass, _amountInBundle);
        _logger.Log($"Purchased {_amountInBundle} {_buffClass}'s");
      }
    }

    private void OnDestroy()
    {
      if (purchaseButton == null) return;
      purchaseButton.onClick.RemoveListener(OnPurchaseClicked);
    }
  }
}
