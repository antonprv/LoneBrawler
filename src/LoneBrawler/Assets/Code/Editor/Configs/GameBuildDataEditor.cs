// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using Code.Editor.Common;

using UnityEditor;

namespace Code.Editor.Configs
{
  [CustomEditor(typeof(GameBuildData))]
  [ManualSaveInspector]
  public class GameBuildDataEditor : ManualSaveEditor
  {
    protected override void DrawInspector()
    {
      DrawDefaultInspectorWithManualSave();
    }
  }
}
