// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Services.StaticDataService.Interfaces;

using UnityEngine;

namespace Code.Infrastructure.Services.StaticDataService
{
  public class StaticDataService : IStaticDataService
  {
    public IPlayerStaticDataService PlayerData { get; private set; }

    public StaticDataService()
    {
      PlayerData = RootContext.Resolve<IPlayerStaticDataService>();
    }
  }
}
