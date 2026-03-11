// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.Time;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Utils.Visuals
{
  public class FramerateManager : ZenjexBehaviour
  {
    public bool showFPS = true;

    private float _deltaTime = 0.0f;

    [Zenjex] private readonly ITimeService _timeService;
    [Zenjex] private readonly IBuildConfigSubservice _build;

    private bool _isInitialized;

    protected override void OnAwake()
    {
      base.OnAwake();

      _isInitialized = true;
    }

    void Start() => Application.targetFrameRate = 120;

    void Update()
    {
      if (!_isInitialized) return;

      if (_build.IsDevelopment())
      {
        _deltaTime += (_timeService.UnscaledDeltaTime - _deltaTime) * 0.1f;
      }
    }

    void OnGUI()
    {
      if (!_isInitialized) return;

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
