// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Player;

using UnityEngine;

namespace Code.Gameplay.Features.UI
{
  public class PlayerUI : MonoBehaviour
  {
    public HealthBar healthBar;

    private PlayerHealth _playerHealth;

    public void Construct(PlayerHealth playerHealth)
    {
      _playerHealth = playerHealth;
      _playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDestroy() =>
      _playerHealth.OnHealthChanged -= UpdateHealthBar;

    public void UpdateHealthBar() =>
      healthBar.SetValue(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
  }
}
