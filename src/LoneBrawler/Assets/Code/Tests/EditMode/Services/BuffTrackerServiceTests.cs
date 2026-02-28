// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.SaveData;
using Code.Data.SaveData.Buffs;
using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Features.Buffs;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.BuffService;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using NSubstitute;

using NUnit.Framework;

namespace Code.Tests.EditMode.Services
{
  [TestFixture]
  public class BuffTrackerServiceTests
  {
    private BuffTrackerService _service;
    private IBuffFactory _buffFactory;
    private IPlayerReader _playerReader;
    private IGameLog _logger;

    [SetUp]
    public void SetUp()
    {
      _buffFactory = Substitute.For<IBuffFactory>();
      _playerReader = Substitute.For<IPlayerReader>();
      _logger = Substitute.For<IGameLog>();
      _service = new BuffTrackerService(_buffFactory, _playerReader, _logger);
    }

    private static BuffBase CreateFakeBuff(
      BuffClassName className,
      BuffActivationType activationType,
      BuffState state)
      => new FakeBuffBase(className, activationType, state);

    #region Tests

    [Test]
    public void AddBuff_ThenGetPlayerBuffs_ContainsBuff()
    {
      var buff = CreateFakeBuff(BuffClassName.SpeedBuff, BuffActivationType.Duration, BuffState.Active);
      _service.AddBuff(buff, BuffClassName.SpeedBuff);
      var result = _service.GetPlayerBuffs(BuffClassName.SpeedBuff);
      Assert.That(result, Has.Count.EqualTo(1));
      Assert.That(result[0], Is.SameAs(buff));
    }

    [Test]
    public void AddBuff_MultipleSameClass_AllAreListed()
    {
      var buff1 = CreateFakeBuff(BuffClassName.HealthBuff, BuffActivationType.Constant, BuffState.Active);
      var buff2 = CreateFakeBuff(BuffClassName.HealthBuff, BuffActivationType.Constant, BuffState.Active);
      _service.AddBuff(buff1, BuffClassName.HealthBuff);
      _service.AddBuff(buff2, BuffClassName.HealthBuff);
      Assert.That(_service.GetPlayerBuffs(BuffClassName.HealthBuff), Has.Count.EqualTo(2));
    }

    [Test]
    public void GetPlayerBuffs_UnknownClass_ReturnsEmpty()
    {
      var result = _service.GetPlayerBuffs(BuffClassName.RageBuff);
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void RemoveBuff_RemovesFromList()
    {
      var buff = CreateFakeBuff(BuffClassName.SpeedBuff, BuffActivationType.Duration, BuffState.Active);
      _service.AddBuff(buff, BuffClassName.SpeedBuff);
      _service.RemoveBuff(buff, BuffClassName.SpeedBuff);
      Assert.That(_service.GetPlayerBuffs(BuffClassName.SpeedBuff), Is.Empty);
    }

    [Test]
    public void RemoveBuff_NonExistentKey_DoesNotThrow()
    {
      var buff = CreateFakeBuff(BuffClassName.GodBuff, BuffActivationType.Duration, BuffState.Active);
      Assert.DoesNotThrow(() => _service.RemoveBuff(buff, BuffClassName.GodBuff));
    }

    [Test]
    public void WriteToProgress_DisabledBuffs_NotSaved()
    {
      var progress = CreateProgress();
      _service.AddBuff(
        CreateFakeBuff(BuffClassName.SpeedBuff, BuffActivationType.Duration, BuffState.Disabled),
        BuffClassName.SpeedBuff);
      _service.WriteToProgress(progress);
      Assert.That(progress.BuffsRegistry.PlayerBuffs, Is.Empty);
    }

    [Test]
    public void WriteToProgress_ActiveBuffs_AreSaved()
    {
      var progress = CreateProgress();
      _service.AddBuff(
        CreateFakeBuff(BuffClassName.RageBuff, BuffActivationType.Duration, BuffState.Active),
        BuffClassName.RageBuff);
      _service.WriteToProgress(progress);
      Assert.That(progress.BuffsRegistry.PlayerBuffs, Has.Count.EqualTo(1));
      Assert.That(progress.BuffsRegistry.PlayerBuffs[0].ClassName, Is.EqualTo(BuffClassName.RageBuff));
    }

    [Test]
    public void WriteToProgress_PassiveBuff_IsSaved()
    {
      var progress = CreateProgress();
      _service.AddBuff(
        CreateFakeBuff(BuffClassName.HealthBuff, BuffActivationType.Constant, BuffState.Passive),
        BuffClassName.HealthBuff);
      _service.WriteToProgress(progress);
      Assert.That(progress.BuffsRegistry.PlayerBuffs, Has.Count.EqualTo(1));
    }

    [Test]
    public void WriteToProgress_ClearsExistingEntriesBeforeWriting()
    {
      var progress = CreateProgress();
      progress.BuffsRegistry.PlayerBuffs.Add(new BuffSaveEntry { ClassName = BuffClassName.DamageBuff });
      progress.BuffsRegistry.PlayerBuffs.Add(new BuffSaveEntry { ClassName = BuffClassName.GodBuff });
      _service.WriteToProgress(progress);
      Assert.That(progress.BuffsRegistry.PlayerBuffs, Is.Empty);
    }

    #endregion

    #region Helper methods

    private static GameProgress CreateProgress()
    {
      var playerData = Substitute.For<IPlayerDataSubervice>();
      playerData.MaxHealth.Returns(100f);
      playerData.MovementSpeed.Returns(5f);
      playerData.RotationSpeed.Returns(3f);
      playerData.AttackDamage.Returns(10f);
      playerData.AttackRange.Returns(5f);
      playerData.AttackRadius.Returns(2f);
      playerData.MaxEnemiesHit.Returns(3);
      return new GameProgress(playerData, "Level_01");
    }

    #endregion

    #region FakeBuffBase

    private sealed class FakeBuffBase : BuffBase
    {
      public FakeBuffBase(BuffClassName className, BuffActivationType activationType, BuffState state)
        : base(className, activationType, state)
      {
      }
    }

    #endregion
  }
}
