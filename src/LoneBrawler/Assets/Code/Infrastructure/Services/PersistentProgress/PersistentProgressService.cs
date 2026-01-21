// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data;

namespace Code.Infrastructure.Services.PersistentProgress
{
  public class PersistentProgressService : IPersistentProgressService
  {
    public PlayerProgress Progress { get; set; }
  }
}
