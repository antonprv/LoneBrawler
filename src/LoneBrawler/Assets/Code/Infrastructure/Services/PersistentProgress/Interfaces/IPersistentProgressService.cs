// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;

namespace Code.Infrastructure.Services.PersistentProgress.Interfaces
{
  public interface IPersistentProgressService
  {
    public GameProgress Progress { get; set; }
  }
}
