// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Common.Extensions.Logging;
using Code.Gameplay.Save;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;

using NSubstitute;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.PlayMode.Common
{
  /// <summary>
  /// PlayMode tests for SaveComponent.
  /// Verify: SaveProgress invocation, Logger.Log invocation, proper functioning
  /// after initialization via Construct.
  /// </summary>
  public class SaveComponentTests
  {
    private GameObject _go;
    private SaveComponent _saveComponent;
    private IGameLog _logger;
    private ISaveLoadService _saveLoad;
    private IPersistentProgressService _progressService;

    [SetUp]
    public void SetUp()
    {
      _go = new GameObject("SaveComponent_Test");
      _saveComponent = _go.AddComponent<SaveComponent>();

      _logger = Substitute.For<IGameLog>();
      _saveLoad = Substitute.For<ISaveLoadService>();
      _progressService = Substitute.For<IPersistentProgressService>();

      _saveComponent.Construct(_logger, _saveLoad, _progressService);
    }

    [TearDown]
    public void TearDown()
    {
      Object.Destroy(_go);
    }

    #region Save

    [UnityTest]
    public IEnumerator Save_CallsSaveProgress()
    {
      _saveComponent.Save();
      yield return null;

      _saveLoad.Received(1).SaveProgress();
    }

    [UnityTest]
    public IEnumerator Save_CallsLoggerLog()
    {
      _saveComponent.Save();
      yield return null;

      _logger.Received(1).Log(Arg.Any<string>());
    }

    [UnityTest]
    public IEnumerator Save_CalledMultipleTimes_SavesEachTime()
    {
      _saveComponent.Save();
      _saveComponent.Save();
      _saveComponent.Save();
      yield return null;

      _saveLoad.Received(3).SaveProgress();
    }

    #endregion

    #region Construct

    [UnityTest]
    public IEnumerator Construct_DoesNotCallSaveImmediately()
    {
      yield return null;
      _saveLoad.DidNotReceive().SaveProgress();
    }

    #endregion

    #region Destruction

    [UnityTest]
    public IEnumerator Destroy_DoesNotThrow()
    {
      Assert.DoesNotThrow(() => Object.DestroyImmediate(_go));
      yield return null;
    }

    #endregion
  }
}
