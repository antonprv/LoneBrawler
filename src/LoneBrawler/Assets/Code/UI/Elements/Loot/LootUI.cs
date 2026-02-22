// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.
using Code.Infrastructure.Services.LootTracker.Interfaces;

using R3;

using TMPro;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Loot
{
  public class LootUI : ZenjexBehaviour
  {
    public TextMeshProUGUI soulsText;
    public CanvasGroup canvasGroup;
    public float updateFlickerSpeed = 0.5f;
    public int updateFlickerAmount = 4;

    [Zenjex] private ILootTrackerService _lootTracker;
    private CompositeDisposable _disposables;

    protected override void OnAwake()
    {
      _disposables = new CompositeDisposable();

      _lootTracker.SoulsRP
        .Subscribe(HandleValueChanged)
        .AddTo(_disposables);
    }

    private void HandleValueChanged(int souls)
    {
      soulsText.text = souls.ToString();
      LeanTween
        .alphaCanvas(canvasGroup, 0, updateFlickerSpeed)
        .setLoopPingPong()
        .loopCount = updateFlickerAmount;
    }

    private void OnDestroy() => _disposables?.Dispose();
  }
}
