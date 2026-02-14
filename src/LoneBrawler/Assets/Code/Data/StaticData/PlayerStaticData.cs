// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Data.StaticData
{
  [UnityEngine.CreateAssetMenu(fileName = "PlayerStaticData", menuName = "StaticData/PlayerStaticData")]
  public class PlayerStaticData : UnityEngine.ScriptableObject
  {
    [UnityEngine.Range(1f, 699f)] public float PlayerMaxHealth = 100f;

    [UnityEngine.Range(1f, 699f)] public float PlayerAttackDamage = 1f;

    [UnityEngine.Range(0.1f, 1f)] public float PlayerAttackRange = 0.8f;

    [UnityEngine.Range(0.1f, 1f)] public float PlayerAttackRadius = 0.7f;


    [UnityEngine.Range(1, 50)] public int PlayerMaxEnemiesHit = 3;

  }
}

