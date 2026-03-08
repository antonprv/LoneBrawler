// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using Code.Data.StaticData.Manifests;
using Code.Editor.Common;
using Code.Editor.Common.Manifests.Drawers;

using UnityEditor;

using UnityEngine;

namespace Code.Editor.StaticData.Audio
{
  [CustomEditor(typeof(LevelMusicManifestStaticData))]
  public class LevelMusicDataEditor : ManualSaveEditor
  {
    private SerializedProperty     _playlistsByLevel;
    private SceneDropdownKeyDrawer _keyDrawer;

    private void OnEnable()
    {
      _playlistsByLevel = serializedObject.FindProperty(nameof(LevelMusicManifestStaticData.PlaylistsByLevel));
      _keyDrawer        = new SceneDropdownKeyDrawer("Playlist");
    }

    protected override void OnDisable()
    {
      _keyDrawer.ClearCache();
      base.OnDisable();
    }

    protected override void DrawInspector()
    {
      EditorGUILayout.Space(6);

      _keyDrawer.DrawDictionaryWithCustomKeys(
        _playlistsByLevel,
        new GUIContent("Playlists By Level")
      );
    }
  }
}
#endif
