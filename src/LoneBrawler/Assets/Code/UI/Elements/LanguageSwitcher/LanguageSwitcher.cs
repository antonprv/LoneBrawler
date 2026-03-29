// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Infrastructure.Services.Localisation.Names;
using Code.Infrastructure.Services.LocalisationService;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.Types;

using R3;

using TMPro;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.LanguageSwitcher
{
  public class LanguageSwitcher : ZenjexBehaviour
  {
    public TMP_Dropdown languageDropdown;

    private const int _russianOption = 0;
    private const int _englishOption = 1;

    [Zenjex] private readonly IGameStateMachine _stateMachine;
    [Zenjex] private readonly ILocalisationService _localisation;

    protected override void OnAwake()
    {
      base.OnAwake();

      if (_stateMachine.GetCurrentStateType() != StateType.MainMenu)
      {
        gameObject.SetActive(false);
        return;
      }

      SetCorrectLanguage();

      languageDropdown.onValueChanged.AddListener(HandleValueChanged);
    }

    private void SetCorrectLanguage()
    {
      var lang = _localisation.GetCurrentLanguage();
      int index = lang == LanguageNames.Russian ? _russianOption : _englishOption;

      languageDropdown.SetValueWithoutNotify(index);
      HandleValueChanged(index);
    }

    private void OnDestroy() =>
      languageDropdown.onValueChanged.RemoveListener(HandleValueChanged);

    private void HandleValueChanged(int languageIndex)
    {
      if (languageIndex == _russianOption)
        _localisation.ChangeLanguage(LanguageNames.Russian);
      else if (languageIndex == _englishOption)
        _localisation.ChangeLanguage(LanguageNames.English);
    }
  }
}
