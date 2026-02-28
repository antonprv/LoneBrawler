// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;
using Code.Infrastructure.Services.PersistentProgress;

using NSubstitute;

using NUnit.Framework;

namespace Code.Tests.EditMode.Services
{
  [TestFixture]
  public class PersistentProgressServiceTests
  {
    [Test]
    public void Progress_DefaultIsNull()
    {
      var service = new PersistentProgressService();
      Assert.That(service.Progress, Is.Null);
    }

    [Test]
    public void Progress_CanBeSet()
    {
      var service = new PersistentProgressService();
      var mockPlayerData = Substitute.For<Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice.IPlayerDataSubervice>();
      mockPlayerData.MaxHealth.Returns(100f);
      mockPlayerData.MovementSpeed.Returns(5f);
      mockPlayerData.RotationSpeed.Returns(3f);
      mockPlayerData.AttackDamage.Returns(10f);
      mockPlayerData.AttackRange.Returns(5f);
      mockPlayerData.AttackRadius.Returns(2f);
      mockPlayerData.MaxEnemiesHit.Returns(3);

      var progress = new GameProgress(mockPlayerData, "Level_01");
      service.Progress = progress;
      Assert.That(service.Progress, Is.SameAs(progress));
    }
  }
}
