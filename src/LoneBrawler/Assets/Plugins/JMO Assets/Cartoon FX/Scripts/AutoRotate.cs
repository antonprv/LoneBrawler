// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

public class AutoRotate : MonoBehaviour
{
  // Rotation speed & axis
  public Vector3 rotation;

  // Rotation space
  public Space space = Space.Self;

  void Update()
  {
    this.transform.Rotate(rotation * Time.deltaTime, space);
  }
}
