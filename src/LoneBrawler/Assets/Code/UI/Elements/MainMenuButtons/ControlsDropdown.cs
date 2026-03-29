// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Types;
using Code.Data.StaticData.Configs.Types;
using Code.Infrastructure.Services.Localisation.Names;
using Code.Infrastructure.Services.LocalisationService;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.Types;
using Code.UI.Services.PlatformControls.Interfaces;

using Cysharp.Threading.Tasks;

using R3;

using TMPro;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.MainMenuButtons
{
  public class ControlsDropdown : ZenjexBehaviour
  {
    public TMP_Dropdown dropdown;

    public string mobileControlsRU = "Сенсорное управление";
    public string mobileControlsEN = "Mobile Controls";

    public string pcControlsRU = "Управление с клавиатуры";
    public string pcControlsEN = "PC Controls";

    [Zenjex] private readonly IPlatformControls _platformControls;
    [Zenjex] private readonly ILocalisationService _localisation;
    [Zenjex] private readonly IBuildConfigSubservice _buildConfig;
    [Zenjex] private readonly IGameStateMachine _stateMachine;

    private const int _mobileControls = 0;
    private const int _pcControls = 1;

    private readonly CompositeDisposable _disposables = new();

    protected override void OnAwake()
    {
      base.OnAwake();

      SetHideConditions();

      dropdown.onValueChanged.AddListener(OnConrolsSwitched);

      _localisation.LanguageRP
        .Subscribe(language => LocaliseOptions(language))
        .AddTo(_disposables);
    }

    private void OnDestroy() => _disposables.Dispose();

    private void SetHideConditions()
    {
      if (_buildConfig.TargetPlatform != TargetPlatform.WebGL)
        gameObject.SetActive(false);

      if (_stateMachine.GetCurrentStateType() != StateType.MainMenu)
        gameObject.SetActive(false);
    }

    private void LocaliseOptions(string language)
    {
      switch (language)
      {
        case LanguageNames.Russian:
          dropdown.options[_mobileControls].text = mobileControlsRU;
          dropdown.options[_pcControls].text = pcControlsRU;
          dropdown.RefreshShownValue();
          break;
        case LanguageNames.English:
          dropdown.options[_mobileControls].text = mobileControlsEN;
          dropdown.options[_pcControls].text = pcControlsEN;
          dropdown.RefreshShownValue();
          break;
        default:
          break;
      }
    }

    private void OnConrolsSwitched(int selection)
    {
      switch (selection)
      {
        case _mobileControls:
          _platformControls.SetScheme(ControlScheme.Mobile);
          break;
        case _pcControls:
          _platformControls.SetScheme(ControlScheme.PC);
          break;
        default:
          break;
      }
    }
  }
}
