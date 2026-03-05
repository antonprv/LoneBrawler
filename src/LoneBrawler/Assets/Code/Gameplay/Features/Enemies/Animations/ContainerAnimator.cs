// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Infrastructure.Services.Time;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Enemies.Animations
{
  public class ContainerAnimator : ZenjexBehaviour, IAnimator
  {
    public float jumpUpHeight = 0.1f;
    public float jumpSpeed = 0.125f;

    [Zenjex] private readonly ITimeService _timeService;

    public void PlayHit() => JumpAnimation();
    public void PlayDeath() => JumpAnimation();

    private void JumpAnimation()
    {
      LeanTween
        .moveLocalY(gameObject, jumpUpHeight, jumpSpeed * _timeService.DeltaAt100FPS)
        .setEase(LeanTweenType.easeOutCubic)
        .setOnComplete(() =>
        {
          LeanTween
          .moveLocalY(gameObject, -jumpUpHeight, jumpSpeed * _timeService.DeltaAt100FPS)
          .setEase(LeanTweenType.easeOutCubic);
        });
    }

    public void PlayPointAttack() { }
  }
}
