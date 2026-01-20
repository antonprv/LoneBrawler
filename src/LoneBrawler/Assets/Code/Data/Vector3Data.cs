// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using System;

namespace Code.Data
{
  [Serializable]
  public class Vector3Data
  {
    float X;
    float Y;
    float Z;

    public Vector3Data(float x, float y, float z)
    {
      X = x;
      Y = y;
      Z = z;
    }
  }
}
