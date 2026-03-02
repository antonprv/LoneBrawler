// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.UI.Windows.Types;

namespace Code.UI.Windows
{
  public class InventoryWindow : WindowBase
  {
    public override void Construct(IPersistentProgressService progressService, ConstructorContext context) =>
      base.Construct(progressService, context);

    protected override void InjectDependencies() { }

    protected override void Initialize()
    {
      base.Initialize();
    }

    protected override void SubscribeUpdates()
    {
      base.SubscribeUpdates();
    }

    protected override void Cleanup()
    {
      base.Cleanup();
    }
  }
}
