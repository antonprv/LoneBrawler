// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using NSubstitute;

namespace Code.Tests.PlayMode.Helpers
{
  internal static class TestHelpers
  {
    /// <summary>
    /// Creates GameProgress with given maxHealth and currentHealth.
    /// Other parameters are filled with reasonable default values.
    /// </summary>
    public static GameProgress CreateProgress(float maxHealth, float currentHealth)
    {
      var playerData = Substitute.For<IPlayerDataSubervice>();
      playerData.MaxHealth.Returns(maxHealth);
      playerData.MovementSpeed.Returns(5f);
      playerData.RotationSpeed.Returns(3f);
      playerData.AttackDamage.Returns(10f);
      playerData.AttackRange.Returns(5f);
      playerData.AttackRadius.Returns(2f);
      playerData.MaxEnemiesHit.Returns(3);

      var progress = new GameProgress(playerData, "TestLevel");
      progress.PLayerState.MaxHealth = maxHealth;
      progress.PLayerState.CurrentHealth = currentHealth;
      return progress;
    }
  }
}
