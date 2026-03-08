// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;

using Code.Editor.Common;

using UnityEditor;

namespace Code.Editor.StaticData.Music
{
  [CustomEditor(typeof(MusicPlaylist))]
  public class MusicPlaylistEditor : ManualSaveEditor
  {
    protected override void DrawInspector() =>
      DrawDefaultInspectorWithManualSave();
  }
}
