// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.SaveData;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.UI.Windows.Types;

using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Windows
{
  public class WindowBase : MonoBehaviour
  {
    public Button closeWindow;

    protected IPersistentProgressService PersistentProgress;
    protected GameProgress Progress => PersistentProgress.Progress;

    protected ConstructorContext ConstructorContext;

    public virtual void Construct(IPersistentProgressService progressService, ConstructorContext context)
    {
      PersistentProgress = progressService;
      ConstructorContext = context;

      InjectDependencies();
      
      Initialize();
      SubscribeUpdates();
    }

    private void Awake() => OnAwake();

    private void OnAwake() =>
      closeWindow.onClick.AddListener(() => Destroy(gameObject));

    private void OnDestroy() => Cleanup();

    protected virtual void InjectDependencies() { }
    protected virtual void SubscribeUpdates() { }
    protected virtual void Initialize() { }
    protected virtual void Cleanup() { }
  }
}
