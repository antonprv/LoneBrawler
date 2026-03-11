// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.UI;
using Code.UI.Factory.Interfaces;
using Code.UI.Windows.Types;

using UnityEngine.UI;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.UI.Windows
{
  public class WindowBase : ZenjexBehaviour
  {
    public Button closeWindow;

    [Zenjex] protected readonly IUIFactory uIFactory;

    protected WindowTypeId windowTypeId;

    protected ConstructorContext ConstructorContext;

    protected Button _openButton;

    public virtual void Construct(
      ConstructorContext context,
      Button openButton
      )
    {
      ConstructorContext = context;

      if (openButton != null)
        _openButton = openButton;
      
      SetWindowType();

      Initialize();
      SubscribeUpdates();
    }

    protected virtual void SetWindowType() => windowTypeId = WindowTypeId.None;

    protected override void OnAwake()
    {
      base.OnAwake();
      closeWindow.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnDestroy() => Cleanup();

    protected virtual void Initialize()
    {
      if (_openButton != null)
        _openButton.interactable = true;
    }
    protected virtual void SubscribeUpdates() { }
    protected virtual void Cleanup()
    {
      uIFactory?.OpenWindows.Remove(windowTypeId);

      if (closeWindow != null)
        closeWindow.onClick.RemoveListener(OnCloseButtonClicked);
    }

    protected virtual void OnCloseButtonClicked() => Destroy(gameObject);
  }
}
