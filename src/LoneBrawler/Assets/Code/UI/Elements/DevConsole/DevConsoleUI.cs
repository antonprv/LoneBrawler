// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.Extensions.ReflexExtensions;
using Code.Infrastructure.Installer;
using Code.Infrastructure.Installer.Interfaces;
using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

using UnityEngine;

namespace Code.UI.Elements.DevConsole
{
  public class DevConsoleUI : MonoBehaviour, IGameInstanceComponent
  {
    [Header("Settings")]
    [SerializeField] private int _maxHistoryLines = 10;

    [Header("History Navigation")]
    [SerializeField] private float _navigationDeadzone = 0.5f;
    [SerializeField] private float _navigationCooldown = 0.2f;

    [Header("Mobile safe area")]
    [SerializeField] private float _safeAreaLeftOffset = 5f;
    [SerializeField] private float _safeAreaRightOffset = 10f;
    [SerializeField] private bool _useSafeAreas = true;

    [Header("Visual Settings")]
    [SerializeField] private int _outputFontSize = 20;
    [SerializeField] private int _inputFontSize = 18;


    private IDevConsole Console =>
      __internalConsole__ ??= RootContext.Resolve<IDevConsole>();
    private IBuildConfigSubservice BuildConfig =>
      __internalBuildConfig__ ??= RootContext.Resolve<IStaticDataService>().BuildConfig;
    private IInputService InputService =>
      __internalInputService__ ??= RootContext.Resolve<IInputService>();

    private bool _isVisible;
    private string _inputText = "";
    private Vector2 _scrollPosition;
    private readonly System.Collections.Generic.List<string> _commandHistory = new();
    private int _historyIndex = -1;
    private string _currentInput = "";

    private GUIStyle _boxStyle;
    private GUIStyle _inputStyle;
    private GUIStyle _outputStyle;
    private GUIStyle _labelStyle;
    private GUIStyle _buttonStyle;

    private bool _isTestPlatform;
    private bool _isMobilePlatform;
    private TouchScreenKeyboard _keyboard;

    private IDevConsole __internalConsole__;
    private IBuildConfigSubservice __internalBuildConfig__;
    private IInputService __internalInputService__;

    // Navigation state
    private float _lastNavigationTime;
    private bool _navigationAxisReleased = true;
    private GameInstance _gameInstance;

    public void RegisterGameInstance(GameInstance gameInstance) =>
      _gameInstance = gameInstance;

    public void DelayedAwake()
    {
      _isTestPlatform = Application.platform == RuntimePlatform.Android
        || Application.platform == RuntimePlatform.IPhonePlayer
        || Application.platform == RuntimePlatform.WindowsEditor
        || Application.platform == RuntimePlatform.LinuxEditor
        || Application.platform == RuntimePlatform.OSXEditor;

      _isMobilePlatform = Application.platform == RuntimePlatform.Android
        || Application.platform == RuntimePlatform.IPhonePlayer;
    }

    private void Update()
    {
      if (!BuildConfig.IsDevelopment())
        return;

      // Toggle console
      if (InputService.IsConsoleButtonPressed())
        ToggleConsole();

      if (_isVisible)
      {
        HandleHistoryNavigationViaInputService();
        HandleCommandSubmit();
        HandleMobileKeyboard();
      }
    }

    private void OnGUI()
    {
      if (!BuildConfig.IsDevelopment())
        return;

      InitializeStyles();

      if (_isVisible)
        DrawConsole();
    }

    private void ToggleConsole()
    {
      Console.Toggle();
      _isVisible = Console.IsEnabled;

      if (_isVisible)
      {
        _inputText = "";
        // Auto-scroll to bottom when opening
        _scrollPosition.y = float.MaxValue;

        // Open mobile keyboard automatically
        if (_isMobilePlatform)
        {
          _keyboard = TouchScreenKeyboard.Open(_inputText, TouchScreenKeyboardType.Default, false, false, false, false);
        }
      }
      else
      {
        if (!_isMobilePlatform) return;
        // Close mobile keyboard
        if (_keyboard != null && TouchScreenKeyboard.visible)
        {
          _keyboard.active = false;
          _keyboard = null;
        }
      }
    }

    private void InitializeStyles()
    {
      if (_boxStyle != null)
        return;

      _boxStyle = new GUIStyle(GUI.skin.box)
      {
        normal = { background = MakeTexture(2, 2, new Color(0, 0, 0, 0.85f)) }
      };

      _inputStyle = new GUIStyle(GUI.skin.textField)
      {
        fontSize = _inputFontSize,
        normal = { textColor = Color.white },
        focused = { textColor = Color.white }
      };

      _outputStyle = new GUIStyle(GUI.skin.label)
      {
        fontSize = _outputFontSize,
        normal = { textColor = Color.white },
        wordWrap = true,
        richText = true
      };

      _labelStyle = new GUIStyle(GUI.skin.label)
      {
        fontSize = 14,
        normal = { textColor = Color.white },
        wordWrap = true
      };

      _buttonStyle = new GUIStyle(GUI.skin.button)
      {
        fontSize = 32,
        fontStyle = FontStyle.Bold,
        normal =
        {
          background = MakeTexture(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.8f)),
          textColor = Color.white
        },
        hover =
        {
          background = MakeTexture(2, 2, new Color(0.3f, 0.3f, 0.3f, 0.9f)),
          textColor = Color.cyan
        },
        active =
        {
          background = MakeTexture(2, 2, new Color(0.4f, 0.4f, 0.4f, 1.0f)),
          textColor = Color.yellow
        }
      };
    }

    private Rect GetConsoleRootRect()
    {
      // PC / Editor / WebGL — full screen
      if (!_isTestPlatform || !_useSafeAreas)
      {
        return new Rect(0, 0, Screen.width, Screen.height);
      }

      // Mobile — Safe Area
      Rect safeArea = Screen.safeArea;

      // Screen.safeArea in scren coordinates is botttom to top,
      // but OnGUI — is top to bottom, so we need to convert Y
      float y = Screen.height - safeArea.yMax;

      return new Rect(
        safeArea.x + _safeAreaLeftOffset,
        y,
        safeArea.width - _safeAreaRightOffset,
        safeArea.height
      );
    }

    private void DrawConsole()
    {
      Rect rootRect = GetConsoleRootRect();

      float consoleHeight = rootRect.height * 0.4f;

      Rect consoleRect = new Rect(
        rootRect.x,
        rootRect.y,
        rootRect.width,
        consoleHeight
      );

      GUILayout.BeginArea(consoleRect, _boxStyle);

      string headerText = _isMobilePlatform
        ? "Developer Console (Tap button to close)"
        : "Developer Console (Press Console key to close)";

      GUILayout.Label(headerText, _labelStyle);
      GUILayout.Space(5);

      float scrollHeight = _isMobilePlatform
        ? consoleHeight - 120
        : consoleHeight - 100;

      _scrollPosition = GUILayout.BeginScrollView(
        _scrollPosition,
        GUILayout.Height(scrollHeight)
      );

      foreach (string message in Console.GetMessages())
      {
        GUILayout.Label(message, _outputStyle);
      }

      GUILayout.EndScrollView();

      if (!_isMobilePlatform)
      {
        GUILayout.BeginHorizontal();

        GUI.SetNextControlName("ConsoleInput");
        _inputText = GUILayout.TextField(
          _inputText,
          _inputStyle,
          GUILayout.Height(30)
        );

        if (_isVisible && Event.current.type == EventType.Layout)
        {
          GUI.FocusControl("ConsoleInput");
        }

        if (GUILayout.Button("Submit", GUILayout.Width(80), GUILayout.Height(30)))
        {
          SubmitCommand();
        }

        GUILayout.EndHorizontal();
      }
      else
      {
        GUILayout.Label($"Input: {_inputText}", _inputStyle, GUILayout.Height(30));

        if (GUILayout.Button("Done", GUILayout.Height(40)))
        {
          SubmitCommand();
        }
      }

      GUILayout.EndArea();
    }


    private void HandleCommandSubmit()
    {
      // Use Input Service for submit
      if (InputService.IsConsoleSubmitPressed())
      {
        SubmitCommand();
      }
    }

    private void SubmitCommand()
    {
      if (!string.IsNullOrWhiteSpace(_inputText))
      {
        ExecuteCommand(_inputText);
        _inputText = "";

        // Auto-scroll to bottom after command execution
        _scrollPosition.y = float.MaxValue;
      }

      // Reopen keyboard for mobile
      if (_isMobilePlatform && _isVisible)
      {
        _keyboard = TouchScreenKeyboard.Open(_inputText, TouchScreenKeyboardType.Default, false, false, false, false);
      }
    }

    private void HandleMobileKeyboard()
    {
      if (!_isMobilePlatform || _keyboard == null)
        return;

      // Update input text from keyboard
      if (_keyboard.status == TouchScreenKeyboard.Status.Visible)
      {
        _inputText = _keyboard.text;
      }

      // Handle keyboard cancel
      if (_keyboard.status == TouchScreenKeyboard.Status.Canceled)
      {
        _keyboard = null;
      }
    }

    private void ExecuteCommand(string command)
    {
      Console.ExecuteCommand(command);

      // Add to command history (for navigation)
      _commandHistory.Add(command);
      if (_commandHistory.Count > _maxHistoryLines)
      {
        _commandHistory.RemoveAt(0);
      }
      _historyIndex = -1;
    }

    private void HandleHistoryNavigationViaInputService()
    {
      // History navigation only for PC
      if (_isTestPlatform)
        return;

      float historyAxis = InputService.GetConsoleHistoryAxis();

      // Check if axis is beyond deadzone
      if (Mathf.Abs(historyAxis) < _navigationDeadzone)
      {
        _navigationAxisReleased = true;
        return;
      }

      // Check cooldown and if axis was released
      if (!_navigationAxisReleased || Time.time - _lastNavigationTime < _navigationCooldown)
        return;

      _navigationAxisReleased = false;
      _lastNavigationTime = Time.time;

      // Navigate history
      if (historyAxis > 0) // Up
      {
        NavigateHistoryUp();
      }
      else if (historyAxis < 0) // Down
      {
        NavigateHistoryDown();
      }
    }

    private void NavigateHistoryUp()
    {
      if (_commandHistory.Count == 0)
        return;

      if (_historyIndex == -1)
      {
        _currentInput = _inputText;
        _historyIndex = _commandHistory.Count - 1;
      }
      else if (_historyIndex > 0)
      {
        _historyIndex--;
      }

      _inputText = _commandHistory[_historyIndex];
    }

    private void NavigateHistoryDown()
    {
      if (_historyIndex == -1)
        return;

      _historyIndex++;
      if (_historyIndex >= _commandHistory.Count)
      {
        _historyIndex = -1;
        _inputText = _currentInput;
      }
      else
      {
        _inputText = _commandHistory[_historyIndex];
      }
    }

    private Texture2D MakeTexture(int width, int height, Color color)
    {
      Color[] pixels = new Color[width * height];
      for (int i = 0; i < pixels.Length; i++)
        pixels[i] = color;

      Texture2D texture = new Texture2D(width, height);
      texture.SetPixels(pixels);
      texture.Apply();
      return texture;
    }
  }
}
