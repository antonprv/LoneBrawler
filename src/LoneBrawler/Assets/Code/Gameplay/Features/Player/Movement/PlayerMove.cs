// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Domain.DataTypes;
using Code.Common.FastMath;
using Code.Data.SaveData;
using Code.Data.SaveData.Types;
using Code.External.Infrastructure.Unity;
using Code.Gameplay.Features.Player.Movement.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.PersistentProgress.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Player.Movement
{
  public class PlayerMove : ZenjexBehaviour, IPlayerMove, IProgressReader, IProgressWriter
  {
    public CharacterController CharacterController;


    [Zenjex] private readonly IInputService _inputService;
    [Zenjex] private readonly ITimeService _timeService;
    [Zenjex] private readonly IPlayerDataSubervice _playerData;

    private IAttacker _attacker;

    private Camera _camera;
    private bool _isAttacking;
    private bool _isMovementEnabled;
    private Vector3 _horizontalMovement;
    private Vector3 _velocity;

    private float _rotationSpeed;
    private float _movementSpeed;

    public void Construct(IAttacker attacker)
    {
      _attacker = attacker;

      _attacker.OnAttacking += HandleAttacking;
      _attacker.OnAttackFinished += HandleAttackFinished;

      _movementSpeed = _playerData.MovementSpeed;
      _rotationSpeed = _playerData.RotationSpeed;
    }

    public void Warp(Vector3 to)
    {
      Warp(
        new TransformData(
          to.ToVector3Data(),
          transform.rotation.ToQuatData(),
          transform.localScale.ToVector3Data()
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

      if (!_isAttacking && _inputService.Axis.sqrMagnitude > FMath.KINDA_SMALL_NUMBER)
      {
        ScreenVectorToWorld();
        Rotate();
      }

      _velocity = _horizontalMovement * _movementSpeed * _timeService.DeltaTime;

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
        _rotationSpeed * _timeService.DeltaTime
      );
    }

    private bool IsMovementForbidden() => !CharacterController.enabled || !_isMovementEnabled;

    public void WriteToProgress(GameProgress playerProgress)
    {
      playerProgress.PlayerWorldData.TransformOnLevel =
      new TransformOnLevel(transform.ToTransformData(), CurrentScene());

      playerProgress.PlayerStats.MovementSpeed = _movementSpeed;
      playerProgress.PlayerStats.RotationSpeed = _rotationSpeed;
    }

    public void ReadProgress(GameProgress playerProgress)
    {
      if (CanReadProgress(playerProgress))
      {
        TransformData savedTransform = playerProgress.CurrentTransform;
        if (savedTransform != null)
          Warp(to: savedTransform);
      }
    }

    private void Warp(TransformData to)
    {
      CharacterController.enabled = false;
      transform.ApplyTransformData(to);
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
