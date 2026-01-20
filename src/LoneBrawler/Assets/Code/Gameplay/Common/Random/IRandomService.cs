// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Gameplay.Common.Random
{
  public interface IRandomService
  {
    public float Range(float inclusiveMin, float inclusiveMax);
    public int Range(int inclusiveMin, int exclusiveMax);
  }
}
