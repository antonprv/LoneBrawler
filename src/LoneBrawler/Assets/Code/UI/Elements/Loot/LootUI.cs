// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.LootTracker.Interfaces;

using TMPro;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Loot
{
  public class LootUI : ZenjexBehaviour
  {
    public TextMeshProUGUI textMeshPro;
    public CanvasGroup canvasGroup;

    public float updateFlickerSpeed = 0.5f;
    public int updateFlickerAmount = 4;

    [Zenjex] private ILootTrackerService _lootTracker;

    protected override void OnAwake()
    {
      textMeshPro.text = _lootTracker.Souls.ToString();
      _lootTracker.OnValueChanged += HandleValueChanged;
    }

    private void HandleValueChanged()
    {
      textMeshPro.text = _lootTracker.Souls.ToString();
      LeanTween
        .alphaCanvas(canvasGroup, 0, updateFlickerSpeed)
        .setLoopPingPong()
        .loopCount = updateFlickerAmount;
    }

    private void OnDestroy() =>
      _lootTracker.OnValueChanged -= HandleValueChanged;
  }
}
