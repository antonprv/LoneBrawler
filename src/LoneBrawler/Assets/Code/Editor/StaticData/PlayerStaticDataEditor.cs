// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Editor.Utils;

using UnityEditor;

namespace Code.Editor.StaticData
{
  [CustomEditor(typeof(PlayerStaticData))]
  public class PlayerStaticDataEditor : ManualSaveEditor
  {
    protected override void DrawInspector()
    {
      DrawDefaultInspectorWithManualSave();
    }
  }
}
