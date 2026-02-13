// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections;

using Code.Gameplay.Utils.ActorComponents.Interfaces;

using UnityEngine;

namespace Code.Gameplay.Utils.ActorComponents
{
  public class AsyncStartMonoBehaviour : MonoBehaviour, IManualStart
  {
    public bool IsInitialized { get; private set; }

    public void ManualStart()
    {
      StartCoroutine(LaunchStart());
    }

    private IEnumerator LaunchStart()
    {
      yield return new WaitForFixedUpdate();
      AsyncStart();
      IsInitialized = true;
    }

    private void Update()
    {
      if (!IsInitialized) return;

      VerifiedUpdate();
    }

    protected virtual void AsyncStart() { }
    protected virtual void VerifiedUpdate() {}
  }
}
