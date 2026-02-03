// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

namespace Code.Editor.Tools
{
  public class EditorTools
  {
    [MenuItem("Tools/Clear PlayerSave")]
    public static void ClearPrefs()
    {
      PlayerPrefs.DeleteAll();
      PlayerPrefs.Save();

      Debug.Log("Cleared all player save data.");
    }

    [MenuItem("Tools/Test game")]
    public static void TestGame()
    {
      string initialScenePath = "Assets/Scenes/Initial.unity";
      SceneAsset initialScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(initialScenePath);

      if (initialScene != null && !EditorApplication.isPlaying)
      {
        // Temporarily start from Initial scene, but not make it default.
        EditorSceneManager.OpenScene(initialScenePath, OpenSceneMode.Single);
        EditorApplication.EnterPlaymode();
        Debug.Log("Launching game from Initial scene...");
      }
      else
      {
        Debug.LogError("Failed to load the Initial scene or game is already running.");
      }
    }
  }
}
