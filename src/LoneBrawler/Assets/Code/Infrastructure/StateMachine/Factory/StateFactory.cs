// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.StateMachine.States.Interfaces;

using Reflex.Core;

namespace Code.Infrastructure.StateMachine.Factory
{
  public class StateFactory
  {
    private readonly Container _container;

    /// <summary>
    /// Initializes states and injects all dependencies in their constructors.
    /// </summary>
    public StateFactory(Container container) => _container = container;

    public T CreateState<T>() where T : IGameExitableState =>
        _container.Resolve<T>();
  }
}
