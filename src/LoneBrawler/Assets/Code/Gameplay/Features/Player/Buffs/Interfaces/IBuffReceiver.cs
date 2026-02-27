// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.Buff;
using Code.Gameplay.Features.Buffs;

using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Features.Player.Buffs.Interfaces
{
  public interface IBuffReceiver
  {
    void ConsumeBuff(BuffBase buff);
    UniTaskVoid ReceiveBuff(BuffClassName className, int amount);
  }
}