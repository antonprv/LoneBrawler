// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.Services.LocalisationService
{
  public interface ILocalisationService
  {
    void ChangeLanguage(string language);
    string GetCurrentLanguage();
    void Initialize();
  }
}