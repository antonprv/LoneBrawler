// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common.Time;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.Gameplay.Common.Visuals
{
  public class FramerateManager : MonoBehaviour
  {
    public bool showFPS = true;

    private float _deltaTime = 0.0f;

    private ITimeService _timeService;
    private IStaticDataService _staticDataService;
    private IBuildConfigSubservice _build;

    private void Awake()
    {
      _timeService = RootContext.Resolve<ITimeService>();
      _staticDataService = RootContext.Resolve<IStaticDataService>();

      _build = _staticDataService.BuildConfig;
    }

    void Start()
    {
      Application.targetFrameRate = 120;
    }

    void Update()
    {
      if (_build.IsDevelopment())
      {
        _deltaTime += (_timeService.UnscaledDeltaTime - _deltaTime) * 0.1f;
      }
    }

    void OnGUI()
    {
      if (_build.IsDevelopment())
      {
        if (!showFPS) return;

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(w - 120, 10, 100, 30); // Right upper corner

        style.alignment = TextAnchor.MiddleRight;
        style.fontSize = 40;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

        float msec = _deltaTime * 1000.0f;
        float fps = 1.0f / _deltaTime;
        string text = $"{msec:F1} ms ({fps:F0}) FPS";

        GUI.Label(rect, text, style);
      }
    }
  }
}
