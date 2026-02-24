// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace YG.Insides
{
  public partial class YGSendMessage : MonoBehaviour
  {
    private void Start()
    {
      YG2.StartInit();
    }

    public void GetDataInvoke()
    {
      YG2.GetDataInvoke();
    }
  }
}