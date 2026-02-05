// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Collections.Generic;

using Code.Data.StaticData.Types;

using UnityEngine;

namespace Code.Data.StaticData
{
  [CreateAssetMenu(fileName = "WindowStaticData",
  menuName = "StaticData/WindowStaticData")]
  public class WindowStaticData : ScriptableObject
  {
    public List<WindowConfig> Configs;
  }
}
