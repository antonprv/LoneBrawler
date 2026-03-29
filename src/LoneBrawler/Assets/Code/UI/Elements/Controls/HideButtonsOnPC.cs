// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData.Types;
using Code.UI.Services.PlatformControls.Interfaces;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.Controls
{
  public class HideButtonsOnPC : ZenjexBehaviour
  {
    public GameObject[] onScreenButtons;

    [Zenjex] private readonly IPlatformControls _platformControls;

    protected override void OnAwake()
    {
      base.OnAwake();

      SetScheme(_platformControls.GetCachedScheme());

      _platformControls.ControlSchemeRP
        .Skip(1)
        .Subscribe(scheme => SetScheme(scheme))
        .AddTo(this.GetCancellationTokenOnDestroy());
    }

    private void SetScheme(ControlScheme scheme)
    {
      if (scheme == ControlScheme.PC)
        foreach (var button in onScreenButtons)
          button.SetActive(false);
      else if (scheme == ControlScheme.Mobile)
        foreach (var button in onScreenButtons)
          button.SetActive(true);
    }
  }
}
