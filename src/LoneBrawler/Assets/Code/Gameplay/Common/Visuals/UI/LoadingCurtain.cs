using System.Collections;

using Code.Common.Extensions.Async;
using Code.Gameplay.Common.Math;

using UnityEngine;

namespace Code.Gameplay.Common.Visuals.UI
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
      while (LoadingScreen.alpha > Constants.KINDA_SMALL_NUMBER)
      {
        LoadingScreen.alpha -= 0.03f;
        yield return new WaitForSeconds(0.03f);
      }
      gameObject.SetActive(false);
    }
  }
}
