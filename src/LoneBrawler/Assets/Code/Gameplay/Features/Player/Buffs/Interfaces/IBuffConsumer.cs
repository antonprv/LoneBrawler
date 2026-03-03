// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Types.Buff;

namespace Code.Gameplay.Features.Player.Buffs.Interfaces
{
  public interface IBuffConsumer
  {
    void ConsumeBuff(BuffClassName buffClass);
  }
}
