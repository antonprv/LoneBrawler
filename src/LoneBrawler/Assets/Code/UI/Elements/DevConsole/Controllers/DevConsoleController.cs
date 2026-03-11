// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.UI.Elements.DevConsole.Model;
using Code.UI.Elements.DevConsole.Services;
using Code.UI.Elements.DevConsole.View;
using Code.UI.Elements.DevConsole.ViewModel;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Core;
using Zenjex.Extensions.Injector;

namespace Code.UI.Elements.DevConsole.Controllers
{
  public class DevConsoleController : ZenjexBehaviour
  {
    [Header("History")]
    [SerializeField] private int _maxHistoryLines = 10;

    [Header("Navigation")]
    [SerializeField] private float _navigationDeadzone = 0.5f;
    [SerializeField] private float _navigationCooldown = 0.2f;

    [Header("Safe Area")]
    [SerializeField] private float _safeAreaLeftOffset = 5f;
    [SerializeField] private float _safeAreaRightOffset = 10f;
    [SerializeField] private bool _useSafeAreas = true;

    [Header("Visual")]
    [SerializeField] private int _outputFontSize = 20;
    [SerializeField] private int _inputFontSize = 18;

    [Zenjex] private readonly IBuildConfigSubservice _buildConfig;
    [Zenjex] private readonly IDevConsole _console;
    [Zenjex] private readonly IInputService _inputService;

    private ConsoleViewModel _viewModel;
    private ConsoleRenderer _renderer;
    private bool _isInitialized;

    protected override void OnAwake()
    {
      base.OnAwake();

      InitializeComponents();

      _isInitialized = true;
    }

    private void Update()
    {
      if (!_isInitialized) return;

      if (!IsDevelopmentBuild())
        return;

      HandleToggle();

      if (_viewModel.IsVisible)
        _viewModel.HandleInput();
    }

    private void OnGUI()
    {
      if (!_isInitialized) return;

      if (!IsDevelopmentBuild())
        return;

      if (!_viewModel.IsVisible)
      {
        if (Event.current.type == EventType.Layout)
          GUI.FocusControl(null);
        return;
      }

      string inputText = _viewModel.InputText;
      _renderer.Render(
        _viewModel.Messages,
        ref inputText,
        _viewModel.IsVisible,
        OnSubmitCommand);
      _viewModel.InputText = inputText;
    }

    private void InitializeComponents()
    {
      PlatformService platform = CreatePlatformService();
      ConsoleStyles styles = CreateStyles();

      _viewModel = CreateViewModel(platform);
      _renderer = new ConsoleRenderer(styles, platform);
    }

    private PlatformService CreatePlatformService() =>
      new PlatformService(
        _safeAreaLeftOffset,
        _safeAreaRightOffset,
        _useSafeAreas
        );

    private ConsoleStyles CreateStyles() =>
      new ConsoleStyles(_outputFontSize, _inputFontSize);

    private ConsoleViewModel CreateViewModel(PlatformService platform)
    {
      ConsoleState state = new ConsoleState();
      CommandHistory history = new CommandHistory(_maxHistoryLines);
      MobileKeyboard keyboard = new MobileKeyboard(platform.IsMobile);

      InputService input = new InputService(
        _inputService,
        platform.IsMobile,
        _navigationDeadzone,
        _navigationCooldown);

      return new ConsoleViewModel(
        _console,
        state,
        history,
        keyboard,
        input,
        platform);
    }

    private void HandleToggle()
    {
      if (_viewModel.IsTogglePressed())
      {
        _viewModel.ToggleConsole();

        if (_viewModel.IsVisible)
          ScrollToBottom();
      }
    }

    private void OnSubmitCommand()
    {
      _viewModel.SubmitCommand();
      ScrollToBottom();
    }

    private void ScrollToBottom()
    {
      _renderer.ScrollToBottom(
        _viewModel.Messages,
        _viewModel.GetViewHeight());
    }

    private bool IsDevelopmentBuild() =>
      _buildConfig.IsDevelopment();
  }
}
