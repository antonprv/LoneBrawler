// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Configs;
using Code.Editor.Common;

using UnityEditor;

namespace Code.Editor.Configs
{
  [CustomEditor(typeof(GameBuildData))]
  public class GameBuildDataEditor : ManualSaveEditor
  {
    protected override void DrawInspector() =>
      DrawDefaultInspectorWithManualSave();
  }
}
