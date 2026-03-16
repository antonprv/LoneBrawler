// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.Localisation.Names;

using YG;

namespace Code.Infrastructure.Services.LocalisationService
{
  public class LocalisationService : ILocalisationService
  {
    private string _currentLanguage;

    public void Initialize() => YG2.onCorrectLang += OnСhangeLang;

    public string GetCurrentLanguage()
    {
      if (!string.IsNullOrEmpty(_currentLanguage))
        return _currentLanguage;
      else
        return YG2.lang;
    }

    public void ChangeLanguage(string language)
    {
      YG2.SwitchLanguage(language);
      _currentLanguage = language;
    }

    private void OnСhangeLang(string lang)
    {
      if (lang != LanguageNames.Russian && lang != LanguageNames.English)
      {
        YG2.lang = LanguageNames.English;
        _currentLanguage = lang;
      }
      else
        _currentLanguage = lang;
    }
  }
}
