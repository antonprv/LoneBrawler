// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Data;

namespace Code.Infrastructure.Services.SaveLoad
{
  public interface ISaveLoadService
  {
    public void SaveProgress();
    public PlayerProgress LoadProgress();
  }
}
