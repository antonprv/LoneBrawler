// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Collections;

using Code.Gameplay.Common;
using Code.Gameplay.Common.Visuals.Particles;

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
    private IParticleSmoothFade _smoothStop;

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

    public event Action OnCollected;

    private void Awake()
    {
      _smoothStop = lootAuraFX.GetComponent<IParticleSmoothFade>();
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

    private void DisableLootItem() =>
      lootItem.SetActive(false);

    private void EaseOutLootAura() =>
      _smoothStop.TriggerStop();

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
