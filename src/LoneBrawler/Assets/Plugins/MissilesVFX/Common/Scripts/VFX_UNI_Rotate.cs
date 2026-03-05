// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

public class VFX_UNI_Rotate : MonoBehaviour
{
  public float RotationSpeed = 120.0f;

  void Update()
  {
    transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
  }
}
