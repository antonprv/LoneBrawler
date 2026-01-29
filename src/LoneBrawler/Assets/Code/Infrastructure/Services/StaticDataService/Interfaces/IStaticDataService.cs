// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.Services.StaticDataService.Interfaces
{
  public interface IStaticDataService
  {
    IPlayerStaticDataService PlayerData { get; }
  }
}
