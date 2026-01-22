// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Data
{
  [Serializable]
  public sealed class QuatData
  {
    public float X;
    public float Y;
    public float Z;
    public float W;

    public QuatData(float x, float y, float z, float w)
    {
      X = x;
      Y = y;
      Z = z;
      W = w;
    }
  }
}
