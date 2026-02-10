// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Player.Movement.Interfaces;
using Code.Infrastructure.Services.DevConsole.Interfaces;
using Code.Infrastructure.Services.DevConsole.Types;
using Code.Infrastructure.Services.PlayerProvider.Interfaces;

using UnityEngine;

namespace Code.Infrastructure.Services.DevConsole.Commands.Gameplay
{
  public class PlayerWarpCommand : IConsoleCommand
  {
    private readonly IDevConsole _console;
    private readonly IPlayerReader _playerReader;

    private GameObject _player;
    private IPlayerMove _playerMove;

    public string CommandName => "warp_player";
    public string Description => "Warps player to set coordinates. Usage: warp_player <x> <y> <z>";

    public PlayerWarpCommand(IDevConsole console, IPlayerReader playerReader)
    {
      _console = console;

      _playerReader = playerReader;
    }

    public void Execute(string[] args)
    {
      if (args.Length < 3)
      {
        _console.AddMessage(Description, ConsoleMessageType.Warning);
        return;
      }

      if (float.TryParse(args[0], out float x) &&
          float.TryParse(args[1], out float y) &&
          float.TryParse(args[2], out float z))
      {

        if (TryGetPlayerMove())
          WarpPlayer(x, y, z);
        else
          _console.AddMessage("Player is null", ConsoleMessageType.Error);
      }
    }

    private void WarpPlayer(float x, float y, float z)
    {
      _playerMove.Warp(new Vector3(x, y, z));
      _console.AddMessage($"Warped to ({x}, {y}, {z})", ConsoleMessageType.Success);
    }

    private bool TryGetPlayerMove()
    {
      _player = _playerReader.GetPlayer();
      if (_player == null)
        return false;

      _playerMove = _player.GetComponent<IPlayerMove>();

      return true;
    }
  }
}
