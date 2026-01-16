// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using SimpleInputNamespace;

using UnityEngine;

public class HideJoystickOnPC : MonoBehaviour
{
  public GameObject Joystick;

  private void Awake()
  {
    RuntimePlatform platform = Application.platform;

    if (platform == RuntimePlatform.Android || Application.isEditor)
    {
      Joystick.SetActive(true);
    }
    else
    {
      Joystick.SetActive(false);
    }
  }
}
