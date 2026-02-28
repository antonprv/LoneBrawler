// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using NSubstitute;

namespace Code.Tests.EditMode.SaveData
{
  internal static class SaveDataTestHelpers
  {
    public static IPlayerDataSubervice MakePlayerData(
        float maxHealth = 100f, float speed = 5f, float rotSpeed = 3f,
        float damage = 10f, float range = 5f, float radius = 2f, int maxEnemies = 3)
    {
      var m = Substitute.For<IPlayerDataSubervice>();
      m.MaxHealth.Returns(maxHealth);
      m.MovementSpeed.Returns(speed);
      m.RotationSpeed.Returns(rotSpeed);
      m.AttackDamage.Returns(damage);
      m.AttackRange.Returns(range);
      m.AttackRadius.Returns(radius);
      m.MaxEnemiesHit.Returns(maxEnemies);
      return m;
    }
  }
}
