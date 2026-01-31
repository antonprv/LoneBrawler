// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Gameplay.Common.NPCInterfaces.DamageSystem
{
  public struct EnemyAttackValues
  {
    public float Range { get; private set; }
    public float Radius { get; private set; }
    public float Damage { get; private set; }
    public int MaxHit { get; private set; }
    public float Cooldown { get; private set; }
    public float TurnSpeed { get; private set; }

    public EnemyAttackValues(
      float range,
      float radius,
      float damage,
      int maxHit,
      float cooldown,
      float turnSpeed
      )
    {
      Range = range;
      Radius = radius;
      Damage = damage;
      MaxHit = maxHit;
      Cooldown = cooldown;
      TurnSpeed = turnSpeed;
    }
  }
}
