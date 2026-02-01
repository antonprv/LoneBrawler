// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;

using Code.Common.Extensions.ReflexExtensions;
using Code.Gameplay.Common;
using Code.Gameplay.Common.Visuals.Particles;
using Code.Gameplay.Common.Visuals.UI.PopUp;
using Code.Gameplay.Features.Loot.TrackerService.Interfaces;

using TMPro;

using UnityEngine;

namespace Code.Gameplay.Features.Loot
{
  public class Loot : MonoBehaviour, ILoot
  {
    public GameObject lootItem;
    public GameObject lootAuraFX;
    public GameObject collectedFX;
    public GameObject textPopupParent;
    public TextMeshProUGUI textPopupMesh;
    public TriggerObserver triggerObserver;

    public Vector3 collectedFXSpawnOffset;

    public float destroyDelay;

    private GameObject _spawnedFX;
    private IParticleSmoothStop _smoothStop;

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
    private ILootTrackerService _lootTracker;

    public event Action OnCollected;

    private void Awake()
    {
      _lootTracker = RootContext.Resolve<ILootTrackerService>();

      _smoothStop = lootAuraFX.GetComponent<IParticleSmoothStop>();
      _smoothStop.OnStopped += HandleSmoothStop;

      triggerObserver.ObservedOnTriggerEnter += HandleTriggerEnter;
    }

    private void HandleTriggerEnter(Collider collider)
    {
      triggerObserver.ObservedOnTriggerEnter -= HandleTriggerEnter;

      OnCollected?.Invoke();
      DisableLootItem();
      ShowCollectedFX();
      ShowTextPopup();
      EaseOutLootAura();
    }

    private void DisableLootItem()
    {
      lootItem.SetActive(false);
    }

    private void EaseOutLootAura() => _smoothStop.TriggerStop();

    private void ShowTextPopup()
    {
      textPopupMesh.text = Souls.ToString();
      textPopupParent.SetActive(true);
    }

    private void ShowCollectedFX()
    {
      _spawnedFX = Instantiate(collectedFX,
        gameObject.transform.position + collectedFXSpawnOffset,
        Quaternion.identity
        );
    }

    private void HandleSmoothStop()
    {
      _smoothStop.OnStopped -= HandleSmoothStop;
      StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
      yield return new WaitForSeconds(destroyDelay);
      Destroy(_spawnedFX);
      Destroy(gameObject);
    }
  }
}
