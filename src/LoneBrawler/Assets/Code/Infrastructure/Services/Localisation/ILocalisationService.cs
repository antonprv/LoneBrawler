// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using R3;

namespace Code.Infrastructure.Services.LocalisationService
{
  public interface ILocalisationService
  {
    ReadOnlyReactiveProperty<string> LanguageRP { get; }

    void ChangeLanguage(string language);
    string GetCurrentLanguage();
    void Initialize();
  }
}
