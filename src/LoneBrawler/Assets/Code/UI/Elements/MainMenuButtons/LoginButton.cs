// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;
using UnityEngine.UI;

using YG;

namespace Code.UI.Elements.MainMenuButtons
{
  public class LoginButton : MonoBehaviour
  {
    public Button button;

    private void Awake() => button.onClick.AddListener(YG2.OpenAuthDialog);
  }
}
