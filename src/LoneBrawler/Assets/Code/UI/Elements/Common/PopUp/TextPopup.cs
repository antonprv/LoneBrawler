// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.Time;
using Code.Common.Extensions.ReflexExtensions;

using UnityEngine;

namespace Code.UI.Elements.Utils.PopUp
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
