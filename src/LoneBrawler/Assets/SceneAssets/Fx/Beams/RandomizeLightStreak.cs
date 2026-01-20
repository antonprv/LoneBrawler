// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

public class RandomizeLightStreak : MonoBehaviour
{
  public Vector2 streakWidthRange = new Vector2(2, 2);

  void Awake()
  {
    LineRenderer lr = GetComponent<LineRenderer>();
    Light l = GetComponent<Light>();
    float r = Random.Range(streakWidthRange.x, streakWidthRange.y);

    if (lr != null)
    {
      lr.startWidth = r;
      lr.endWidth = r;
    }

    if (l != null && l.type == LightType.Spot)
    {
      l.spotAngle = r * 3;
    }
  }


}
