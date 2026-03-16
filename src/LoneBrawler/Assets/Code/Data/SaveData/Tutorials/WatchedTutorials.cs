// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Common.CustomTypes.Domain.Collections;
using Code.Data.SaveData.Tutorials.Types;

namespace Code.Data.SaveData.Tutorials
{
  [System.Serializable]
  public class WatchedTutorials
  {
    public HashSetData<TutorialType> Tutorials;

    public WatchedTutorials() => Tutorials = new();
  }
}
