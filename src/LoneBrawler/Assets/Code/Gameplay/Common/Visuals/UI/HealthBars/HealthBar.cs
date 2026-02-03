// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Common.Visuals.UI.HealthBars
{
  public class HealthBar : MonoBehaviour
  {
    public Image ImageCurrent;
    public float speedChange;

    public void SetValue(float HealthCurrent, float HealthMax)
    {
      ImageCurrent.fillAmount = HealthCurrent / HealthMax;
    }
  }
}
