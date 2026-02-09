// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEditor;

namespace Code.Gameplay.LevelTeleport
{
  [CustomEditor(typeof(LevelTeleportMarker))]
  public class LevelTeleportMarkerEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      EditorGUILayout.LabelField("ТЕСТ - Кастомный инспектор работает!");

      serializedObject.Update();
      SerializedProperty levelKeyProperty = serializedObject.FindProperty("LevelKey");
      EditorGUILayout.PropertyField(levelKeyProperty);
      serializedObject.ApplyModifiedProperties();
    }
  }
}
