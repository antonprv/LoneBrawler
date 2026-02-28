// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.StaticData;
using Code.Data.StaticData.Types.Buff;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Shop
{
  public class ShopItemView : ZenjexBehaviour
  {
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI AmountText;
    public Button purchaseButton;

    public int amountInBundle = 1;

    private BuffClassName _buffClass;

    [Zenjex] private readonly IBuffDataSubservice _buffData;
    [Zenjex] private readonly IGameLog _logger;

    public void Initialize(BuffClassName buffClass)
    {
      _buffClass = buffClass;
      SetupButton();
      LoadDataAsync().Forget();
      SetupAmount();
    }

    private void SetupAmount() =>
      AmountText.text = amountInBundle > 1 ?
      amountInBundle.ToString() : string.Empty;

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

      if (priceText == null) return;
      priceText.text = buffData.Cost.ToString();
    }

    private void OnPurchaseClicked()
    {
      // TODO: purchase logic
      _logger.Log($"Attempted purchase of {_buffClass}");
    }

    private void OnDestroy()
    {
      if (purchaseButton == null) return;
      purchaseButton.onClick.RemoveListener(OnPurchaseClicked);
    }
  }
}
