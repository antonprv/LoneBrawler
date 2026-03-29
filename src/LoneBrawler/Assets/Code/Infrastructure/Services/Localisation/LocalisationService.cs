// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Infrastructure.Services.Localisation.Names;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using R3;

using YG;

namespace Code.Infrastructure.Services.LocalisationService
{
  public class LocalisationService : ILocalisationService, IDisposable
  {
    private readonly IBuildConfigSubservice _buildConfig;

    private readonly ReactiveProperty<string> _languageRP = new(LanguageNames.English);
    public ReadOnlyReactiveProperty<string> LanguageRP => _languageRP;

    public LocalisationService(IBuildConfigSubservice buildConfig) =>
      _buildConfig = buildConfig;

    public void Initialize()
    {
      if (_buildConfig.UseCloudSave)
      {
        OnСhangeLang(YG2.lang);
        YG2.onCorrectLang += OnСhangeLang;
      }
      else
        _languageRP.Value = LanguageNames.English;
    }
    public void Dispose() => YG2.onCorrectLang -= OnСhangeLang;

    public string GetCurrentLanguage()
    {
      if (!_buildConfig.UseCloudSave)
        return _languageRP.CurrentValue;

      return YG2.lang;
    }

    public void ChangeLanguage(string language)
    {
      _languageRP.Value = language;

      if (_buildConfig.UseCloudSave)
        YG2.SwitchLanguage(language);
    }

    private void OnСhangeLang(string lang)
    {
      if (lang != LanguageNames.Russian && lang != LanguageNames.English)
      {
        YG2.lang = LanguageNames.English;
        _languageRP.Value = LanguageNames.English;
      }
      else
        _languageRP.Value = lang;
    }
  }
}
