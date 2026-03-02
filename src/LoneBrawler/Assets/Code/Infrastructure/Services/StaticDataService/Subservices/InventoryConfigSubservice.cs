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
  public class InventoryConfigSubservice : IInventoryConfigSubservice
  {
    public int InventorySize { get; private set; }

    public int HotbarSize { get; private set; }

    private static InventorySystemConfig _inventoryConfig;
    private readonly IGameLog _logger;
    private readonly IAssetLoader _assetLoader;

    public InventoryConfigSubservice(IGameLog gameLog, IAssetLoader assetLoader)
    {
      _logger = gameLog;
      _assetLoader = assetLoader;
    }

    public async UniTask LoadSelfAsync()
    {
      if (_inventoryConfig) return;

      _inventoryConfig = await _assetLoader
        .LoadAsync<InventorySystemConfig>
        (StaticDataAddresses.InventoryConfigAddress);

      if (!_inventoryConfig)
        _logger.Log(LogType.Error,
          $"{typeof(InventorySystemConfig)} not found!" +
          $" Make sure it's in Bundles folder with correct path");

      InventorySize = _inventoryConfig.InventorySize;
      HotbarSize = _inventoryConfig.HotbarSize;
    }
  }
}
