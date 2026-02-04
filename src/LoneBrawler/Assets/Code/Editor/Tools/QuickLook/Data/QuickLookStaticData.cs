// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using UnityEngine;

namespace Code.Editor.Tools.QuickLook.Data
{
  [CreateAssetMenu(fileName = "QuickLookStaticData",
  menuName = "StaticData/Editor/QuickLookStaticData")]
  public class QuickLookStaticData : ScriptableObject
  {
    public List<GameObject> Prefabs;
    public List<ScriptableObject> ScriptableObjects;
  }
}
