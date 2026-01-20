// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Input;
using Code.Gameplay.Common.Math;
using Code.Gameplay.Common.Time;

using UnityEngine;

namespace Code.Gameplay.Features.Hero
{
  public class HeroMove : MonoBehaviour
  {
    public CharacterController CharacterController;
    public float MovementSpeed = 4.0f;
    public float RotationSpeed = 12.0f;

    private IInputService _inputService;
    private ITimeService _timeService;
    private Camera _camera;

    private void Awake()
    {
      _inputService = RootContext.Resolve<IInputService>();
      _timeService = RootContext.Resolve<ITimeService>();
    }

    private void Start()
    {
      _camera = Camera.main;
    }

    private void Update()
    {
      Vector3 movementVector = Vector3.zero;

      if (_inputService.Axis.sqrMagnitude > Constants.KINDA_SMALL_NUMBER)
      {
        // Transform screen vector to world vector
        movementVector = _camera.transform.TransformDirection(_inputService.Axis);
        movementVector.y = 0;
        movementVector.Normalize();

        // Smooth rotation towards movement direction
        Quaternion targetRotation = Quaternion.LookRotation(movementVector);

        transform.rotation = Quaternion.Slerp(
          transform.rotation,
          targetRotation,
          RotationSpeed * _timeService.DeltaTime
        );
      }

      movementVector += Physics.gravity;

      CharacterController.Move(MovementSpeed * movementVector * _timeService.DeltaTime);
    }
  }
}
