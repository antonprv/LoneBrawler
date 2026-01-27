// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Features.UI
{
  public class HealthBar : MonoBehaviour
  {
    public Image ImageCurrent;

    public void SetValue(float HealthCurrent, float HealthMax) =>
      ImageCurrent.fillAmount = HealthCurrent / HealthMax;
  }
}
