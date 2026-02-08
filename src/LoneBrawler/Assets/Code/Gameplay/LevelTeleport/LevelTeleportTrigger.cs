// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions.Types;
using Code.Gameplay.Features.Save.Interfaces;
using Code.Gameplay.Services.Time;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
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
    private string _uniqueName;
    private IPersistentProgressService _progressService;
    private IGameStateMachine _stateMachine;
    private ITimeService _timeService;
    private ISaveComponent _saveComponent;
    private bool _triggered;

    public void Construct(
      IPersistentProgressService progressService,
      IGameStateMachine stateMachine,
      ITimeService timeService,
      ISaveComponent saveComponent,
      Coordinates coords,
      Vector3 scale,
      string levelKey,
      string uniqueName
      )
    {
      _progressService = progressService;
      _stateMachine = stateMachine;
      _timeService = timeService;

      _saveComponent = saveComponent;

      _levelKey = levelKey;
      _uniqueName = uniqueName;

      gameObject.transform.SetPositionAndRotation(
        coords.Position, coords.Rotation);

      boxCollider.size = scale;
    }

    private void OnTriggerEnter(Collider other)
    {
      if (_triggered) return;
      _triggered = true;

      UpdateLastTeleportName();
      UpdateLastTeleportTime();
      SaveGame();
      LoadLevel();
    }

    private void SaveGame() => _saveComponent.Save();

    private void UpdateLastTeleportName() =>
      _progressService.Progress.PlayerWorldData.LastTeleportUniqueName = _uniqueName;

    private void UpdateLastTeleportTime() =>
      _progressService.Progress.PlayerWorldData.LastTeleportTimeUTC = _timeService.UtcNow.Ticks;

    private void LoadLevel() =>
      _stateMachine.EnterState<LoadLevelState, string>(_levelKey);
  }

}
