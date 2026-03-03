// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Features.Player.Buffs.Interfaces;
using Code.Gameplay.Utils.ActorComponents;
using Code.Infrastructure.Services.InventoryService.Interfaces;
using Code.Infrastructure.Services.SoulsTracker.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Zenjex.Extensions.Core;

namespace Code.UI.Elements.Shop
{
  public class ShopItemView : MonoBehaviour
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
    private IGameLog _logger;
    private IBuffDataSubservice _buffData;
    private IInventoryService _inventoryService;
    private ISoulsTrackerService _soulsTrackerService;

    private void InjectDependencies()
    {
      _logger = RootContext.Resolve<IGameLog>();
      _buffData = RootContext.Resolve<IBuffDataSubservice>();
      _inventoryService = RootContext.Resolve<IInventoryService>();
      _soulsTrackerService = RootContext.Resolve<ISoulsTrackerService>();
    }

    public void Construct(BuffClassName buffClass)
    {
      InjectDependencies();
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
      nameText.text = buffData.DisplayName;
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
      }
    }

    private void OnDestroy()
    {
      if (purchaseButton == null) return;
      purchaseButton.onClick.RemoveListener(OnPurchaseClicked);
    }
  }
}
