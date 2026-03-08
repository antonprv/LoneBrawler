// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Enemies.Health;
using Code.UI.Elements.Utils.HealthBars;

using R3;

using UnityEngine;

namespace Code.UI.Elements.Enemies
{
  public class EnemyUI : MonoBehaviour
  {
    public HealthBar healthBar;
    public EnemyHealth _enemyHealth;

    private readonly CompositeDisposable _disposables = new();

    private void Awake()
    {
      _enemyHealth.CurrentHealthRP
        .CombineLatest(_enemyHealth.MaxHealthRP, (current, max) => (current, max))
        .Where(pair => pair.max > 0f)
        .Subscribe(pair => healthBar.SetValue(pair.current, pair.max))
        .AddTo(_disposables);
    }

    private void OnDestroy() => _disposables.Dispose();
  }
}
