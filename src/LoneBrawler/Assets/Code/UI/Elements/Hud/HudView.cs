// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.RestartGame.Interfaces;
using Code.Infrastructure.Services.Time;

using R3;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Hud
{
  public class HudView : ZenjexBehaviour, IRestartHandler
  {
    public CanvasGroup hudGroup;

    public float fadeSpeed = 0.5f;

    private const int hudDisappearAlpha = 0;

    [Zenjex] private readonly IRestartGameService _restartGameService;
    [Zenjex] private readonly ITimeService _timeService;

    protected override void OnAwake()
    {
      base.OnAwake();

      _restartGameService.RegisterHandler(this);
    }

    private void OnDestroy() =>
      _restartGameService.UnregisterHandler(this);

    public Observable<Unit> PrepareForRestart()
    {
      var subject = new Subject<Unit>();

      LeanTween
          .alphaCanvas(
            hudGroup,
            hudDisappearAlpha,
            fadeSpeed * _timeService.DeltaAt100FPS
          )
          .setOnComplete(_ =>
            {
              gameObject.SetActive(false);
              subject.OnNext(Unit.Default);
              subject.OnCompleted();
            }
          );

      return subject;
    }
  }
}
