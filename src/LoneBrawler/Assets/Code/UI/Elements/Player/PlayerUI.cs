// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.UI.Elements.Utils.HealthBars;

using R3;

using UnityEngine;

namespace Code.UI.Elements.Player
{
  public class PlayerUI : MonoBehaviour
  {
    public HealthBar healthBar;

    private readonly CompositeDisposable _disposables = new();

    public void Construct(IHealth playerHealth)
    {
      playerHealth.CurrentHealthRP
        .CombineLatest(playerHealth.MaxHealthRP, (current, max) => (current, max))
        .Subscribe(pair => healthBar.SetValue(pair.current, pair.max))
        .AddTo(_disposables);
    }

    private void OnDestroy() => _disposables.Dispose();
  }
}
