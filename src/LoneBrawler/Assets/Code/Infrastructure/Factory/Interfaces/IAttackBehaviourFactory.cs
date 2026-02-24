// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Gameplay.Features.Enemies.Attack.DetailedConfig.Interfaces;
using Code.Gameplay.Utils.NPCInterfaces.DamageSystem;
using Code.Infrastructure.AssetManagement.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Code.Infrastructure.Factory
{
  public interface IAttackBehaviourFactory
  {
    UniTask<IAttackBehaviour> CreateAsync(
      Transform ownerTransform,
      EnemyStaticData staticData,
      AttackPresetStaticData preset,
      IHealth playerHealth,
      int playerLayerMask,
      IAssetLoader assetLoader);
  }
}
