// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.FastMath;
using Code.Gameplay.Audio.Sound;
using Code.Gameplay.Audio.Sound.Types;
using Code.Gameplay.Features.Player.Animations;
using Code.Gameplay.Features.Player.Movement;
using Code.Gameplay.Utils.NPCInterfaces.Animations;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Gameplay.Utils.NPCInterfaces.Lifetime;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.RestartGame.Interfaces;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Player.Health
{
  [RequireComponent(typeof(PlayerAnimator))]
  [RequireComponent(typeof(PlayerMove))]
  public class PlayerDeath : ZenjexBehaviour, IDeath
  {
    public SoundPlayer soundPlayer;
    public bool IsDead { get; private set; }

    public GameObject DeathFX;

    [Zenjex] private readonly IRestartGameService _restartGameService;
    [Zenjex] private readonly IInputService _inputService;

    private IAnimator _animator;

    private IHealth _health;

    private readonly CompositeDisposable _disposables = new();

    public void Construct(IAnimator animator, IHealth health)
    {
      IsDead = false;

      _animator = animator;
      _health = health;

      SubscribeToRP();
    }

    private void SubscribeToRP()
    {
      _health.CurrentHealthRP
        .Skip(1)
        .Where(hp => hp.IsNearlyZero())
        .Subscribe(_ => Die().Forget())
        .AddTo(_disposables);
    }

    private void OnDestroy() => _disposables?.Dispose();

    private async UniTaskVoid Die()
    {
      DeactivateComponents();

      _animator.PlayDeath();
      IsDead = true;

      Instantiate(
        DeathFX,
        transform.position,
        Quaternion.identity
        );

      await soundPlayer.PlaySound(SoundType.Death);
      _inputService.GameInputEnabled = false;
      _restartGameService.RequestRestart();
    }

    private void DeactivateComponents()
    {
      foreach (var component in GetComponents<IDeactivatable>())
        component.Deactivate();
    }
  }
}
