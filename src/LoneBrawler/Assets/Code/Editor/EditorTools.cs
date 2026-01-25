// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

namespace Code.Editor
{
  public class EditorTools
  {
    [MenuItem("Tools/Clear PlayerSave")]
    public static void ClearPrefs()
    {
      PlayerPrefs.DeleteAll();
      PlayerPrefs.Save();
    }

    [MenuItem("Tools/Test game")]
    public static void TestGame()
    {
      EditorSceneManager.playModeStartScene =
        AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Initial.unity");
      EditorApplication.EnterPlaymode();
    }
  }
}
