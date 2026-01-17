// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Common.Time;

using Reflex.Attributes;

using UnityEngine;

namespace Code.Gameplay.Common.Visuals
{
  public class FramerateManager : MonoBehaviour
  {
    public bool showFPS = true;
    private float deltaTime = 0.0f;

    private ITimeService _timeService;

    [Inject]
    private void Construct(ITimeService time)
    {
      _timeService = time;
    }

    void Start()
    {
      Application.targetFrameRate = 120;
    }

    void Update()
    {
      deltaTime += (_timeService.UnscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
      if (!showFPS) return;

      int w = Screen.width, h = Screen.height;

      GUIStyle style = new GUIStyle();

      Rect rect = new Rect(w - 120, 10, 100, 30); // Right upper corner

      style.alignment = TextAnchor.MiddleRight;
      style.fontSize = 40;
      style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

      float msec = deltaTime * 1000.0f;
      float fps = 1.0f / deltaTime;
      string text = $"{msec:F1} ms ({fps:F0}) FPS";

      GUI.Label(rect, text, style);
    }
  }
}
