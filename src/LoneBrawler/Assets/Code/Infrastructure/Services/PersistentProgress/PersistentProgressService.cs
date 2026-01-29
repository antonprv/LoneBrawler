// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;

using Code.Infrastructure.Services.PersistentProgress.Interfaces;

namespace Code.Infrastructure.Services.PersistentProgress
{
  public class PersistentProgressService : IPersistentProgressService
  {
    public GameProgress Progress { get; set; }
  }
}
