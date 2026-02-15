// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Common.Extensions.Logging;
using Code.Common.Extensions.ReflexExtensions;
using Code.Data.StaticData.Configs;
using Code.Data.StaticData.Configs.Types;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class BuildConfigSubservice : IBuildConfigSubservice
  {
    public BuildConfiguration Current { get; private set; }

    public TargetPlatform TargetPlatform { get; private set; }

    private static GameBuildData _buildConfig;
    private IGameLog _logger;
    private IAssetLoader _assetLoader;

    public BuildConfigSubservice()
    {
      _logger = RootContext.Resolve<IGameLog>();
      _assetLoader = RootContext.Resolve<IAssetLoader>();
    }

    public bool IsDevelopment()
    {
      return Current == BuildConfiguration.Development;
    }

    public async Task LoadSelfAsync()
    {
      if (_buildConfig) return;

      _buildConfig = await _assetLoader.LoadAsync<GameBuildData>(StaticDataAddresses.BuildConfigAddress);

      if (!_buildConfig)
        _logger.Log(LogType.Error,
          $"{typeof(GameBuildData)} not found!" +
          $" Make sure it's in a Resources folder with correct path");

      Current = _buildConfig.BuildConfiguration;
      TargetPlatform = _buildConfig.Platform;
    }
  }
}
