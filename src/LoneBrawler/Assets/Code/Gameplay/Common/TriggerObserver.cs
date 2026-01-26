// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

using UnityEngine;

namespace Code.Gameplay.Common
{
  [RequireComponent(typeof(Collider))]
  public class TriggerObserver : MonoBehaviour
  {
    public event Action<Collider> ObservedOnTriggerEnter;
    public event Action<Collider> ObservedOnTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
      ObservedOnTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
      ObservedOnTriggerExit?.Invoke(other);
    }
  }
}
