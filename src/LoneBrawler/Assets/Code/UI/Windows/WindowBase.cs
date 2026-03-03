// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;
using Code.Data.StaticData.Types.UI;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.UI.Elements;
using Code.UI.Factory.Interfaces;
using Code.UI.Windows.Types;

using UnityEngine;
using UnityEngine.UI;

using Zenjex.Extensions.Core;

namespace Code.UI.Windows
{
  public class WindowBase : MonoBehaviour
  {
    public Button closeWindow;
    public virtual WindowTypeId WindowType => WindowTypeId.None;

    protected IPersistentProgressService PersistentProgress;

    protected IUIFactory UIFactory;

    protected GameProgress Progress => PersistentProgress.Progress;

    protected ConstructorContext ConstructorContext;

    protected Button _openButton;

    public virtual void Construct(
      IPersistentProgressService progressService,
      ConstructorContext context,
      Button openButton
      )
    {
      PersistentProgress = progressService;
      ConstructorContext = context;

      _openButton = openButton;

      InjectDependencies();

      Initialize();
      SubscribeUpdates();
    }

    private void Awake() => OnAwake();

    private void OnAwake() =>
      closeWindow.onClick.AddListener(OnCloseButtonClicked);

    private void OnDestroy() => Cleanup();

    protected virtual void InjectDependencies() => UIFactory = RootContext.Resolve<IUIFactory>();
    protected virtual void SubscribeUpdates() { }
    protected virtual void Initialize() { }
    protected virtual void Cleanup()
    {
      UIFactory?.OpenWindows.Remove(WindowType);
      
      if (closeWindow != null)
        closeWindow.onClick.RemoveListener(OnCloseButtonClicked);
    }

    private void OnCloseButtonClicked() => Destroy(gameObject);
  }
}
