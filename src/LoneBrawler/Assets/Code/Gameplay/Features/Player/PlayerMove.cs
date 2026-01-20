// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Math;
using Code.Gameplay.Common.Time;
using Code.Infrastructure.Services.Input;

using UnityEngine;

namespace Code.Gameplay.Features.Player
{
  public class PlayerMove : MonoBehaviour
  {
    public CharacterController CharacterController;
    public float MovementSpeed = 4.0f;
    public float RotationSpeed = 12.0f;

    private IInputService _inputService;
    private ITimeService _timeService;
    private Camera _camera;

    private Vector3 _movementVector;

    private void Awake()
    {
      _inputService = RootContext.Resolve<IInputService>();
      _timeService = RootContext.Resolve<ITimeService>();

      _camera = Camera.main;
    }

    private void Start()
    {
      _camera = Camera.main;
    }

    private void Update()
    {
      RotatePlayer();
      MovePlayer();
    }

    private void MovePlayer()
    {
      _movementVector = Vector3.zero;

      if (_inputService.Axis.sqrMagnitude > Constants.KINDA_SMALL_NUMBER)
      {
        // Transform screen vector to world vector
        _movementVector = _camera.transform.TransformDirection(_inputService.Axis);
        _movementVector.y = 0;
        _movementVector.Normalize();
      }

      _movementVector += Physics.gravity;
      CharacterController.Move(MovementSpeed * _movementVector * _timeService.DeltaTime);
    }

    private void RotatePlayer()
    {
      if (_inputService.Axis.sqrMagnitude > Constants.KINDA_SMALL_NUMBER)
      {
        Quaternion targetRotation = Quaternion.LookRotation(_movementVector);

        transform.rotation = Quaternion.Slerp(
          transform.rotation,
          targetRotation,
          RotationSpeed * _timeService.DeltaTime);
      }
    }
  }
}
