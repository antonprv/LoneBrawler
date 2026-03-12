// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.
using Code.Infrastructure.Services.SoulsTracker.Interfaces;

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

    [Zenjex] private ISoulsTrackerService _lootTracker;
    private CompositeDisposable _disposables = new();

    private int _flickerTweenId = -1;

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

      // Reset previous tween before launching a new one
      if (LeanTween.isTweening(_flickerTweenId))
        LeanTween.cancel(_flickerTweenId);

      // ResetAlpha before tweening
      canvasGroup.alpha = 1f;

      _flickerTweenId = LeanTween
          .alphaCanvas(canvasGroup, 0, updateFlickerSpeed)
          .setLoopPingPong()
          .setLoopCount(updateFlickerAmount)
          .setOnComplete(() => canvasGroup.alpha = 1f) // guaranteeed visibility at the end
          .uniqueId;
    }

    private void OnDestroy() => _disposables?.Dispose();
  }
}
