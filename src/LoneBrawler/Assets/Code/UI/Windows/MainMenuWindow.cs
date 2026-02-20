// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.Logging;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;

using Zenjex.Extensions.Core;

namespace Code.UI.Windows
{
  public class MainMenuWindow : WindowBase
  {
    private IGameLog _logger;

    public override void Construct(IPersistentProgressService progressService) =>
      base.Construct(progressService);
    protected override void InjectDependencies() =>
      _logger = RootContext.Resolve<IGameLog>();

    protected override void Initialize() => CheckPlayerProgress();

    protected override void SubscribeUpdates() => CheckPlayerProgress();

    protected override void Cleanup() => base.Cleanup();

    private void CheckPlayerProgress() =>
      _logger.Log("MainMenu checked player progress...");
  }
}
