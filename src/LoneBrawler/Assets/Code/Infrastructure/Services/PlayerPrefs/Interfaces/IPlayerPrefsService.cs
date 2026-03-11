// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.Services.PlayerPrefs.Interfaces
{
  public interface IPlayerPrefsService
  {
    void DeleteAll();
    string GetString(string payload);
    void Save();
    void SetString(string key, string value);
  }
}
