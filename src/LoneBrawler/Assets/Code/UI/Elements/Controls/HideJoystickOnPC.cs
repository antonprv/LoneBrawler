// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.UI.Elements.Controls
{
  public class HideJoystickOnPC : MonoBehaviour
  {
    public GameObject Joystick;

    private void Awake() => HideIfPCPlatform();

    private void HideIfPCPlatform()
    {
      RuntimePlatform platform = Application.platform;

      if (platform == RuntimePlatform.Android || Application.isEditor)
        Joystick.SetActive(true);
      else
        Joystick.SetActive(false);
    }
  }
}
