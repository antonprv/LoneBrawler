// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Services.LootTracker.Interfaces;

using Code.Common.Extensions.ReflexExtensions;

using TMPro;

using UnityEngine;

namespace Code.UI.Elements.Loot
{
  public class LootUI : MonoBehaviour
  {
    public TextMeshProUGUI textMeshPro;
    public CanvasGroup canvasGroup;

    public float updateFlickerSpeed = 0.5f;
    public int updateFlickerAmount = 4;

    private ILootTrackerService _lootTracker;

    private void Awake()
    {
      _lootTracker = RootContext.Resolve<ILootTrackerService>();
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
