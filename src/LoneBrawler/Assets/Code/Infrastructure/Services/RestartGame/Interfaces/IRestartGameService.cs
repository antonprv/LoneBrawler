// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using R3;

namespace Code.Infrastructure.Services.RestartGame.Interfaces
{
  public interface IRestartGameService
  {
    Observable<Unit> OnRestartRequested { get; }
    IReadOnlyList<IRestartHandler> RestartHandlers { get; }

    void RegisterHandler(IRestartHandler handler);
    void UnregisterHandler(IRestartHandler handler);

    public void RequestRestart();
  }
}
