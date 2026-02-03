// Created by Anston Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;
using Code.Editor.Common;

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
