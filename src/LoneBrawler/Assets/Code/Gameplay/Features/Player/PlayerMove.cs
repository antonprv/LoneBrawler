// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Data;
using Code.Data.DataExtensions;
using Code.Gameplay.Common.Time;
using Code.Infrastructure.Services.Input;
using Code.Infrastructure.Services.PersistentProgress;

using Unity.Burst;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Gameplay.Features.Player
{
  public class PlayerMove : MonoBehaviour, IProgressReader, IProgressWriter
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
      MovePlayer();
    }

    [BurstCompile]
    private void MovePlayer()
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

      if (CharacterController.enabled)
      {
        CharacterController.Move(MovementSpeed * movementVector * _timeService.DeltaTime);
      }
    }

    public void WriteToProgress(PlayerProgress playerProgress) =>
  playerProgress.WorldData.TransformOnLevel =
    new TransformOnLevel(transform.AsTransformData(), CurrentScene());


    public void ReadProgress(PlayerProgress playerProgress)
    {
      if (playerProgress.IsWorldDataValid() &&
        CurrentScene() == playerProgress.CurrentScene)
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
      transform.position = transform.position.AddY(CharacterController.height);
      CharacterController.enabled = true;
    }

    private string CurrentScene() => SceneManager.GetActiveScene().name;
  }
}
