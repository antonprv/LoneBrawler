// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay.Features.Player.Movement.Interfaces;
using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.DevConsole.Interfaces;
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

    public string CommandName => "warp";
    public string Description => "Warps player to set coordinates. Usage: warp <x> <y> <z>";

    public PlayerWarpCommand(IDevConsole console, IPlayerReader playerReader)
    {
      _console = console;

      _playerReader = playerReader;
    }

    public void Execute(string[] args)
    {
      if (args.Length < 3)
      {
        Debug.LogWarning($"[Console] {Description}");
        return;
      }

      if (float.TryParse(args[0], out float x) &&
          float.TryParse(args[1], out float y) &&
          float.TryParse(args[2], out float z))
      {

        if (TryGetPlayerMove())
        {
          WarpPlayer(x, y, z);
        }
        else
        {
          _console.AddMessage($"[Console] Error: player is null", ConsoleMessageType.Error);
        }
      }
    }

    private void WarpPlayer(float x, float y, float z)
    {
      _console.AddMessage($"[Console] Warping to ({x}, {y}, {z})", ConsoleMessageType.Log);
      _playerMove.Warp(new Vector3(x, y, z));
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
