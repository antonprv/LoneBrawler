// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using UnityEngine;

namespace Code.UI.Elements.Controls
{
  public class HideButtonsOnPC : MonoBehaviour
  {
    public List<GameObject> onScreenButtons;

    private void Awake() => HideIfPCPlatform();

    private void HideIfPCPlatform()
    {
      RuntimePlatform platform = Application.platform;

      if (platform == RuntimePlatform.Android
#if UNITY_EDITOR
        || Application.isEditor
#endif
        )

        foreach (var button in onScreenButtons)
          button.SetActive(true);
      else
        foreach (var button in onScreenButtons)
          button.SetActive(false);
    }
  }
}
