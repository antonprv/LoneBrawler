// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.CustomTypes;
using Code.Common.Extensions.CustomTypes.Types;

using Code.Data.SaveData;
using Code.Data.SaveData.Types;
using Code.Gameplay.Features.Player.Movement.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.Time;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Gameplay.Features.Player.Movement
{
  public class PlayerMove : MonoBehaviour, IPlayerMove, IProgressReader, IProgressWriter
  {
    public CharacterController CharacterController;

    public float MovementSpeed = 4.0f;
    public float RotationSpeed = 12.0f;

    private IInputService _inputService;
    private ITimeService _timeService;

    private IAttacker _attacker;

    private Camera _camera;
    private bool _isAttacking;
    private bool _isMovementEnabled;
    private Vector3 _horizontalMovement;
    private Vector3 _velocity;

    public void Construct(IInputService inputService, ITimeService timeService, IAttacker attacker)
    {
      _inputService = inputService;
      _timeService = timeService;
      _attacker = attacker;

      _attacker.OnAttacking += HandleAttacking;
      _attacker.OnAttackFinished += HandleAttackFinished;
    }

    public void Warp(Vector3 to)
    {
      Warp(
        new TransformData(
          to.AsVector3Data(),
          QuatExtensions.Identity(),
          Vector3Extensions.One()
          )
        );
    }

    public void Deactivate()
    {
      CharacterController.enabled = false;
      _isMovementEnabled = false;
      enabled = false;
    }

    public void Activate()
    {
      CharacterController.enabled = true;
      _isMovementEnabled = true;
      enabled = true;
    }

    private void HandleAttacking() => _isAttacking = true;

    private void HandleAttackFinished() => _isAttacking = false;

    private void Start() => _camera = Camera.main;

    private void Update() => MovePlayer();

    private void OnDestroy()
    {
      _attacker.OnAttacking -= HandleAttacking;
      _attacker.OnAttackFinished -= HandleAttackFinished;
    }

    private void MovePlayer()
    {
      if (IsMovementForbidden()) return;

      _horizontalMovement = Vector3.zero;

      if (!_isAttacking && _inputService.Axis.sqrMagnitude > Constants.KINDA_SMALL_NUMBER)
      {
        ScreenVectorToWorld();
        Rotate();
      }

      _velocity = _horizontalMovement * MovementSpeed * _timeService.DeltaTime;

      if (!CharacterController.isGrounded)
        _velocity += Physics.gravity * _timeService.DeltaTime;

      CharacterController.Move(_velocity);
    }

    private void ScreenVectorToWorld()
    {
      _horizontalMovement = _camera.transform.TransformDirection(_inputService.Axis);
      _horizontalMovement.y = 0;
      _horizontalMovement.Normalize();
    }

    private void Rotate()
    {
      Quaternion targetRotation = Quaternion.LookRotation(_horizontalMovement);

      transform.rotation = Quaternion.Slerp(
        transform.rotation,
        targetRotation,
        RotationSpeed * _timeService.DeltaTime
      );
    }

    private bool IsMovementForbidden() => !CharacterController.enabled || !_isMovementEnabled;

    public void WriteToProgress(GameProgress playerProgress) =>
      playerProgress.PlayerWorldData.TransformOnLevel =
      new TransformOnLevel(transform.AsTransformData(), CurrentScene());

    public void ReadProgress(GameProgress playerProgress)
    {
      if (CanReadProgress(playerProgress))
      {
        TransformData savedTransform = playerProgress.CurrentTransform;
        if (savedTransform != null)
        {
          Warp(to: savedTransform);
        }
      }
    }
    private void Warp(TransformData to)
    {
      CharacterController.enabled = false;
      to.ApplyTo(transform);
      CharacterController.enabled = true;
    }

    private bool CanReadProgress(GameProgress playerProgress)
    {
      return playerProgress.IsWorldDataValid()
        && CurrentScene() == playerProgress.CurrentScene
        && SaveIsNewer(playerProgress);
    }

    private static bool SaveIsNewer(GameProgress playerProgress) => playerProgress.PlayerWorldData.LastTeleportTimeUTC < playerProgress.SaveTimeUTC;

    private string CurrentScene() => SceneManager.GetActiveScene().name;
  }
}
