// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.DataExtensions;

using R = UnityEngine.Random;

namespace Code.Gameplay.Common.Random
{
  public class UnityRandomService : IRandomService
  {
    private int _lastInt;
    private bool _hasLastInt;

    private float _lastFloat;
    private bool _hasLastFloat;

    public int Range(int inclusiveMin, int exclusiveMax, bool nonRepeating = false) =>
      nonRepeating
      ? NonRepeating(inclusiveMin, exclusiveMax)
      : R.Range(inclusiveMin, exclusiveMax);

    public float Range(float inclusiveMin, float inclusiveMax, bool nonRepeating = false) =>
      nonRepeating
      ? NonRepeating(inclusiveMin, inclusiveMax)
      : R.Range(inclusiveMin, inclusiveMax);


    private int NonRepeating(int inclusiveMin, int exclusiveMax)
    {
      int count = exclusiveMax - inclusiveMin;
      if (count <= 1)
        return inclusiveMin;

      int value;
      do
      {
        value = UnityEngine.Random.Range(inclusiveMin, exclusiveMax);
      }
      while (_hasLastInt && value == _lastInt);

      _lastInt = value;
      _hasLastInt = true;
      return value;
    }


    private float NonRepeating(float inclusiveMin, float inclusiveMax)
    {
      if (inclusiveMin >= inclusiveMax)
        return inclusiveMin;

      float value;
      do
      {
        value = UnityEngine.Random.Range(inclusiveMin, inclusiveMax);
      }
      while (_hasLastFloat && value.IsNearlyEqual(_lastFloat));

      _lastFloat = value;
      _hasLastFloat = true;
      return value;
    }
  }
}
