// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Gameplay.Features.UI
{
  public class LookAtCamera : MonoBehaviour
  {
    private Camera _mainCamera;

    private void Start() => _mainCamera = Camera.main;

    private void Update()
    {
      Quaternion rotation = _mainCamera.transform.rotation;
      transform.LookAt(transform.position + rotation * Vector3.back, Vector3.up);
    }
  }
}
