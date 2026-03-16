// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.Localisation.Names;
using Code.Infrastructure.Services.LocalisationService;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.Types;

using TMPro;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.LanguageSwitcher
{
  public class LanguageSwitcher : ZenjexBehaviour
  {
    public TMP_Dropdown languageDropdown;

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

      languageDropdown.onValueChanged.AddListener(HandleValueChanged);
    }

    private void OnDestroy() =>
      languageDropdown.onValueChanged.RemoveListener(HandleValueChanged);

    private void HandleValueChanged(int languageIndex)
    {
      if (languageIndex == 0)
        _localisation.ChangeLanguage(LanguageNames.Russian);
      else if (languageIndex == 1)
        _localisation.ChangeLanguage(LanguageNames.English);
    }
  }
}
