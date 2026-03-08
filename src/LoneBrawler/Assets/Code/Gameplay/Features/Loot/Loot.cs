// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;

using Code.Common.UtilityComponents;
using Code.Data.StaticData;
using Code.Gameplay.Features.Loot.Interfaces;
using Code.Gameplay.Utils.Visuals.Particles;
using Code.Infrastructure.AssetManagement.Interfaces;

using Cysharp.Threading.Tasks;

using R3;

using TMPro;

using UnityEngine;
using UnityEngine.AddressableAssets;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Features.Loot
{
  public class Loot : ZenjexBehaviour, ILoot
  {
    [Zenjex] private readonly IAssetLoader _assetLoader;

    public GameObject lootItem;
    public GameObject lootAuraFX;
    public GameObject collectedFX;
    public GameObject textPopupParent;
    public TextMeshProUGUI textPopupMesh;
    public TriggerObserver triggerObserver;

    public Vector3 collectedFXSpawnOffset;

    public float destroyDelay;

    private GameObject _spawnedFX;
    private IParticleSmoothFade _smoothStop;

    private readonly Subject<Unit> _onCollected = new();
    public Observable<Unit> OnCollected => _onCollected;

    public int Souls
    {
      get => _souls;
      set
      {
        if (_isSet) return;
        _isSet = true;
        _souls = value;
      }
    }

    private int _souls;
    private bool _isSet;
    private AssetReferenceGameObject _collectedFXPrefab;

    public void Construct(EnemyStaticData enemyData) =>
      _collectedFXPrefab = enemyData.CollectedFXPrefabReference;

    protected override void OnAwake()
    {
      base.OnAwake();
      _smoothStop = lootAuraFX.GetComponent<IParticleSmoothFade>();

      _smoothStop.OnStopped
        .Take(1)
        .Subscribe(_ => DestroyAfterDelayAsync().Forget());

      triggerObserver.ObservedOnTriggerEnter += HandleTriggerEnter;
    }
 
    private void OnDestroy()
    {
      _onCollected.Dispose();
      triggerObserver.ObservedOnTriggerEnter -= HandleTriggerEnter;
    }

    private void HandleTriggerEnter(Collider collider)
    {
      triggerObserver.ObservedOnTriggerEnter -= HandleTriggerEnter;

      _onCollected.OnNext(Unit.Default);
      _onCollected.OnCompleted();

      DisableLootItem();
      ShowCollectedFX().Forget();
      ShowTextPopup();
      EaseOutLootAura();
    }

    private void DisableLootItem() =>
      lootItem.SetActive(false);

    private void EaseOutLootAura() =>
      _smoothStop.TriggerStop();

    private void ShowTextPopup()
    {
      textPopupMesh.text = Souls.ToString();
      textPopupParent.SetActive(true);
    }

    private async UniTaskVoid ShowCollectedFX()
    {
      _spawnedFX =
        await _assetLoader
        .InstantiateAsync(_collectedFXPrefab, gameObject.transform);

      _spawnedFX.transform
        .SetPositionAndRotation(
        transform.position + collectedFXSpawnOffset,
        Quaternion.identity
        );
    }

    private async UniTaskVoid DestroyAfterDelayAsync()
    {
      await UniTask.Delay(
        TimeSpan.FromSeconds(destroyDelay),
        cancellationToken: this.GetCancellationTokenOnDestroy());

      Destroy(_spawnedFX);
      Destroy(gameObject);
    }
  }
}
