// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Elements.MainMenuButtons
{
	public class SettingsButton : MonoBehaviour
	{
    public Button button;

    public GameObject settingsWindow;

    private void Awake() =>
      button.onClick.AddListener(HandleClicked);

    private void HandleClicked() => settingsWindow.SetActive(true);

    private void OnDestroy()
    {
      if (button != null)
        button.onClick.RemoveListener(HandleClicked);
    }
  }
}
