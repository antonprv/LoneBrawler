// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Windows
{
  public class WindowBase : MonoBehaviour
  {
    public Button closeWindow;

    private void Awake()
    {
      OnAwake();
    }

    private void OnAwake()
    {
      closeWindow.onClick.AddListener(() => Destroy(gameObject));
    }
  }
}
