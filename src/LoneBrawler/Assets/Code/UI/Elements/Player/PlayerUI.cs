// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Player.Health;
using Code.UI.Elements.Utils.HealthBars;

using UnityEngine;

namespace Code.UI.Elements.Player
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
