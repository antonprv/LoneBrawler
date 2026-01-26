// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common;
using Code.Gameplay.Features.Enemies.Animations;

using UnityEngine;

namespace Code.Gameplay.Features.Enemies.Attack
{
  [RequireComponent(typeof(EnemyAnimator))]
  public class Attack : MonoBehaviour
  {
    public EnemyAnimator animator;
    public TriggerObserver triggerObserver;

    private void Awake()
    {

    }

    private void OnDisable()
    {

    }
  }
}
