// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Infrastructure.Services.LootTracker;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using NSubstitute;

using NUnit.Framework;

using R3;

namespace Code.Tests.EditMode.Misc
{
  [TestFixture]
  public class LootTrackerReactiveTests
  {
    [Test]
    public void SoulsRP_FiresOnSubscribe_WithCurrentValue()
    {
      var playerData = Substitute.For<IPlayerDataSubervice>();
      playerData.MaxHealth.Returns(100f);
      playerData.MovementSpeed.Returns(5f);
      playerData.RotationSpeed.Returns(3f);
      playerData.AttackDamage.Returns(10f);
      playerData.AttackRange.Returns(5f);
      playerData.AttackRadius.Returns(2f);
      playerData.MaxEnemiesHit.Returns(3);
      var progress = new Code.Data.SaveData.GameProgress(playerData, "L");
      var progressService = Substitute.For<IPersistentProgressService>();
      progressService.Progress.Returns(progress);

      var service = new LootTrackerService(progressService);

      var values = new List<int>();
      service.SoulsRP.Subscribe(v => values.Add(v));

      service.AddSouls(10);
      service.AddSouls(20);

      Assert.That(values, Contains.Item(10));
      Assert.That(values, Contains.Item(30));
    }
  }
}
