// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Infrastructure.Serialization;
using Code.Data.SaveData;
using Code.Data.StaticData.Configs.Types;
using Code.Infrastructure.Factory.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.SaveLoad.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.StaticDataService.Subservices;
using Code.Infrastructure.Services.Time;

using UnityEngine;

namespace Code.Infrastructure.Services.SaveLoad
{
  public class SaveLoadService : ISaveLoadService
  {
    private const string ProgressKey = "Progress";

    private readonly IPersistentProgressService _persistentProgressService;
    private readonly IGameFactory _gameFactory;
    private readonly ITimeService _timeService;
    private readonly IBuildConfigSubservice _buildConfig;

    public SaveLoadService(
      IPersistentProgressService progressService,
      IGameFactory gameFactory,
      ITimeService timeService,
      IBuildConfigSubservice buildConfig
      )
    {
      _persistentProgressService = progressService;
      _gameFactory = gameFactory;
      _timeService = timeService;
      _buildConfig = buildConfig;
    }

    public void SaveProgress()
    {
      switch (_buildConfig.TargetPlatform)
      {
        case TargetPlatform.None:
          break;
        case TargetPlatform.YandexGames:
          break;
        case TargetPlatform.RuStore:
          SaveLocalProgress();
          break;
        case TargetPlatform.GamePush:
          break;
        case TargetPlatform.ItchIoBrowser:
          SaveLocalProgress();
          break;
        case TargetPlatform.ItchIoDevice:
          SaveLocalProgress();
          break;
        default:
          break;
      }
    }

    private void SaveLocalProgress()
    {
      foreach (IProgressWriter progressWriter in _gameFactory.ProgressWriters)
        progressWriter.WriteToProgress(_persistentProgressService.Progress);

      _persistentProgressService.Progress.SaveTimeUTC = _timeService.UtcNow.Ticks;

      PlayerPrefs.SetString(ProgressKey, _persistentProgressService.Progress.ToSerialized());
    }

    public GameProgress LoadProgress()
    {
      switch (_buildConfig.TargetPlatform)
      {
        case TargetPlatform.None:
          return null;
        case TargetPlatform.YandexGames:
          return null;
        case TargetPlatform.RuStore:
          return LoadLocalProgress();
        case TargetPlatform.GamePush:
          return null;
        case TargetPlatform.ItchIoBrowser:
          return LoadLocalProgress();
        case TargetPlatform.ItchIoDevice:
          return LoadLocalProgress();
        default:
          return null;
      }
    }

    private static GameProgress LoadLocalProgress()
    {
      return PlayerPrefs.GetString(ProgressKey)?.ToDeserialized<GameProgress>();
    }
  }
}
