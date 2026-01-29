// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Health;

using UnityEngine;
using Code.Gameplay.Common.Visuals.UI.HealthBars;

namespace Code.Gameplay.Features.Enemies.UI
{
  public class EnemyUI : MonoBehaviour
  {
    public HealthBar healthBar;
    public EnemyHealth _enemyHealth;

    private void Awake()
    {
      _enemyHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDestroy() =>
      _enemyHealth.OnHealthChanged -= UpdateHealthBar;

    public void UpdateHealthBar() =>
      healthBar.SetValue(_enemyHealth.CurrentHealth, _enemyHealth.MaxHealth);
  }
}
