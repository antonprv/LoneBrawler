// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;
using System.Linq;

using Code.Data.StaticData;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class LevelDataSubservice : ILevelDataSubservice
  {
    private Dictionary<string, LevelStaticData> _levelData;

    public void LoadSelf() => _levelData = Resources
        .LoadAll<LevelStaticData>(StaticDataAddresses.LevelDataPath)
        .ToDictionary(x => x.LevelKey, x => x);

    public LevelStaticData ForLevel(string sceneKey) =>
      _levelData.TryGetValue(sceneKey, out LevelStaticData levelStaticData)
      ? levelStaticData
      : null;
  }
}
