// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Time;

using UnityEngine;

namespace Code.Gameplay.Common.Visuals.UI.PopUp
{
  public class TextPopup : MonoBehaviour
  {
    public CanvasGroup textCanvas;

    public float tweenSpeed = 0.1f;
    public float moveEndPoint;

    private ITimeService _timeService;

    private void Awake() =>
      _timeService = RootContext.Resolve<ITimeService>();

    private void Start()
    {
      Quaternion rotation = Camera.main.transform.rotation;
      transform.LookAt(transform.position + rotation * Vector3.forward, Vector3.up);
    }

    private void OnEnable()
    {
      LeanTween
        .moveLocalY(gameObject, moveEndPoint, tweenSpeed)
        .setEase(LeanTweenType.easeOutCubic);

      LeanTween
        .alphaCanvas(textCanvas, 0f, tweenSpeed)
        .setEase(LeanTweenType.easeInOutCubic);
    }
  }
}
