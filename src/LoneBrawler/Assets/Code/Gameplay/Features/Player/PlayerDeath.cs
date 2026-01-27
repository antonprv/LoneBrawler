// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions;

using UnityEngine;

namespace Code.Gameplay.Features.Player
{
  [RequireComponent(typeof(PlayerAnimator))]
  [RequireComponent(typeof(PlayerMove))]
  [RequireComponent(typeof(PlayerHealth))]
  public class PlayerDeath : MonoBehaviour
  {
    public PlayerAnimator animator;
    public PlayerMove move;
    public PlayerHealth health;

    public bool IsDead {  get; private set; }

    public GameObject DeathFX;

    private void Awake() => IsDead = false;

    private void Start() =>
      health.OnHealthChanged += HandleHealthChanged;

    private void OnDestroy() =>
      health.OnHealthChanged -= HandleHealthChanged;

    private void HandleHealthChanged()
    {
      if (health.CurrentHealth.IsNearlyZero())
        Die();
    }

    private void Die()
    {
      animator.PlayDeath();
      move.enabled = false;

      IsDead = true;

      Instantiate(
        DeathFX,
        transform.position,
        Quaternion.identity
        );
    }
  }
}
