// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.UI.Elements.Common.LoadingScreen.Interfaces;

using Code.Common.FastMath;

using UnityEngine;

namespace Code.UI.Elements.Common.LoadingScreen
{
  public class LoadingCurtain : MonoBehaviour, ILoadScreen
  {
    public CanvasGroup LoadingScreen;

    private void Awake()
    {
      DontDestroyOnLoad(this);
    }

    public void Show()
    {
      gameObject.SetActive(true);
      LoadingScreen.alpha = 1.0f;
    }

    public void Hide() => StartCoroutine(FadeIn());

    private IEnumerator FadeIn()
    {
      while (LoadingScreen.alpha > FMath.KINDA_SMALL_NUMBER)
      {
        LoadingScreen.alpha -= 0.03f;
        yield return new WaitForSeconds(0.03f);
      }
      gameObject.SetActive(false);
    }
  }
}
