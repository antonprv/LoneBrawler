// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using Code.Common.Extensions.Logging;
using Code.Data.StaticData.Configs;
using Code.Infrastructure.AssetManagement.Addresses;
using Code.Infrastructure.AssetManagement.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class MusicConfigSubservice : IMusicConfigSubservice
  {
    public MusicPlayerConfig Confg { get; private set; }

    private MusicPlayerConfig _playerConfig;

    private readonly IAssetLoader _assetLoader;
    private readonly IGameLog _logger;

    public MusicConfigSubservice(IAssetLoader assetLoader, IGameLog gameLog)
    {
      _assetLoader = assetLoader;
      _logger = gameLog;
    }

    public async UniTask LoadSelfAsync()
    {
      if (_playerConfig != null) return;

      _playerConfig =
        await _assetLoader
        .LoadAsync<MusicPlayerConfig>(StaticDataAddresses.MusicPlayerConfigAddress);

      if (!_playerConfig)
        _logger.Log(LogType.Error,
          $"{typeof(MusicPlayerConfig)} not found!" +
          $" Make sure its Addressable address is correct!");

      Confg = _playerConfig;
    }
  }
}
