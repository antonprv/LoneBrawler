// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace YG.Example.DemoScene
{
  public class DemoSceneLoadButton : MonoBehaviour
  {
    public string sceneName;
    public Text textButton;

    public void Setup(string sceneName)
    {
      this.sceneName = sceneName;
      textButton.text = sceneName;
      gameObject.name = sceneName;
    }

    public void LoadScene()
    {
      if (!string.IsNullOrEmpty(sceneName))
      {
        SceneManager.LoadScene(sceneName);
      }
      else
      {
        Debug.LogError("Scene name is empty, cannot load scene.");
      }
    }
  }
}
