// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;
using Code.Infrastructure.Services.LootTracker;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using NSubstitute;

using NUnit.Framework;

using R3;

namespace Code.Tests.EditMode.Services
{
  [TestFixture]
  public class LootTrackerServiceTests
  {
    private LootTrackerService _service;
    private IPersistentProgressService _progressService;
    private GameProgress _progress;

    [SetUp]
    public void SetUp()
    {
      var mockPlayerData = Substitute.For<Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice.IPlayerDataSubervice>();
      mockPlayerData.MaxHealth.Returns(100f);
      mockPlayerData.MovementSpeed.Returns(5f);
      mockPlayerData.RotationSpeed.Returns(3f);
      mockPlayerData.AttackDamage.Returns(10f);
      mockPlayerData.AttackRange.Returns(5f);
      mockPlayerData.AttackRadius.Returns(2f);
      mockPlayerData.MaxEnemiesHit.Returns(3);

      _progress = new GameProgress(mockPlayerData, "Level_01");

      _progressService = Substitute.For<IPersistentProgressService>();
      _progressService.Progress.Returns(_progress);

      _service = new LootTrackerService(_progressService);
    }

    #region AddSouls

    [Test]
    public void AddSouls_IncreasesProgressAmount()
    {
      _service.AddSouls(50);
      Assert.That(_progress.SoulsCollected.Amount, Is.EqualTo(50));
    }

    [Test]
    public void AddSouls_MultipleCalls_Accumulates()
    {
      _service.AddSouls(10);
      _service.AddSouls(20);
      Assert.That(_progress.SoulsCollected.Amount, Is.EqualTo(30));
    }

    [Test]
    public void AddSouls_UpdatesReactiveProperty()
    {
      int observed = 0;
      _service.SoulsRP.Subscribe(v => observed = v);
      _service.AddSouls(75);
      Assert.That(observed, Is.EqualTo(75));
    }

    [Test]
    public void AddSouls_Zero_NoChange()
    {
      _service.AddSouls(0);
      Assert.That(_progress.SoulsCollected.Amount, Is.EqualTo(0));
    }

    #endregion

    #region SpendSouls

    [Test]
    public void SpendSouls_SufficientAmount_DeductsAndReturnsTrue()
    {
      _service.AddSouls(100);
      bool result = _service.SpendSouls(60);
      Assert.That(result, Is.True);
      Assert.That(_progress.SoulsCollected.Amount, Is.EqualTo(40));
    }

    [Test]
    public void SpendSouls_InsufficientAmount_ReturnsFalse()
    {
      _service.AddSouls(30);
      bool result = _service.SpendSouls(50);
      Assert.That(result, Is.False);
      Assert.That(_progress.SoulsCollected.Amount, Is.EqualTo(30)); // unchanged
    }

    [Test]
    public void SpendSouls_ExactAmount_DeductsToZero()
    {
      _service.AddSouls(50);
      bool result = _service.SpendSouls(50);
      Assert.That(result, Is.True);
      Assert.That(_progress.SoulsCollected.Amount, Is.EqualTo(0));
    }

    [Test]
    public void SpendSouls_ZeroBalance_ReturnsFalse()
    {
      bool result = _service.SpendSouls(1);
      Assert.That(result, Is.False);
    }

    [Test]
    public void SpendSouls_UpdatesReactiveProperty()
    {
      _service.AddSouls(100);
      int observed = 0;
      _service.SoulsRP.Subscribe(v => observed = v);
      _service.SpendSouls(40);
      Assert.That(observed, Is.EqualTo(60));
    }

    [Test]
    public void SoulsRP_InitialValue_IsZero()
    {
      Assert.That(_service.SoulsRP.CurrentValue, Is.EqualTo(0));
    }

    #endregion
  }
}
