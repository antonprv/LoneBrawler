// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Data.StaticData.Configs;
using Code.Data.StaticData.Configs.Types;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class BuildConfigSubservice : IBuildConfigSubservice
  {
    public DebugConfiguration Current { get; private set; }

    public TargetPlatform TargetPlatform { get; private set; }

    public bool UseCloudSave { get; private set; }

    public bool UseAddSdk { get; private set; }

    private static GameBuildData _buildConfig;
    private readonly IGameLog _logger;
    private readonly IAssetLoader _assetLoader;

    public BuildConfigSubservice(IGameLog gameLog, IAssetLoader assetLoader)
    {
      _logger = gameLog;
      _assetLoader = assetLoader;
    }

    public bool IsDevelopment() =>
      Current == DebugConfiguration.Development;

    public async UniTask LoadSelfAsync()
    {
      if (_buildConfig) return;

      _buildConfig = await _assetLoader.LoadAsync<GameBuildData>(StaticDataAddresses.BuildConfigAddress);

      if (!_buildConfig)
        _logger.Log(LogType.Error,
          $"{typeof(GameBuildData)} not found!" +
          $" Make sure it's in a Resources folder with correct path");

      Current = _buildConfig.DebugConfiguration;
      TargetPlatform = _buildConfig.Platform;
      UseCloudSave = _buildConfig.UseCloudSave;
      UseAddSdk = _buildConfig.UseAddSdk;
    }
  }
}
