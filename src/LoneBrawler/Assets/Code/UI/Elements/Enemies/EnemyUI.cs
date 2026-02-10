// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Health;
using Code.UI.Elements.Utils.HealthBars;

using UnityEngine;

namespace Code.UI.Elements.Enemies
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
