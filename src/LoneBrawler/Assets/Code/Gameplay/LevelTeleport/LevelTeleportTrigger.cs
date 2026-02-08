// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions.Types;
using Code.Infrastructure.StateMachine.Interfaces;
using Code.Infrastructure.StateMachine.States;

using UnityEngine;

namespace Code.Gameplay.LevelTeleport
{
  [RequireComponent(typeof(BoxCollider))]
  public class LevelTeleportTrigger : MonoBehaviour
  {
    public BoxCollider boxCollider;

    private string _levelKey;

    private IGameStateMachine _stateMachine;
    private bool _triggered;

    public void Construct(
      IGameStateMachine stateMachine,
      Coordinates coords,
      Vector3 scale,
      string levelKey
      )
    {
      _stateMachine = stateMachine;
      _levelKey = levelKey;

      gameObject.transform.SetPositionAndRotation(
        coords.Position, coords.Rotation);

      boxCollider.size = scale;
    }

    private void OnTriggerEnter(Collider other)
    {
      if (_triggered) return;
      _triggered = true;

      _stateMachine.EnterState<LoadLevelState, string>(_levelKey);
    }
  }
}
