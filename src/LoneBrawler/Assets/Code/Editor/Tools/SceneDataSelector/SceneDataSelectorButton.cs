// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using System.IO;
using System.Linq;

using Code.Data.StaticData;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;

using UnityEngine;

namespace Code.Editor.Tools.LevelInspector
{
  [EditorToolbarElement(SceneDataSelectorButton.ID, typeof(SceneView))]
  public sealed class SceneDataSelectorButton : EditorToolbarButton
  {
    public const string ID = "LevelInspector/Button";

    public SceneDataSelectorButton()
    {
      text = "Current Level Data";
      tooltip = "Open the Level Static Data for the currently open level.";

      icon = EditorGUIUtility.IconContent("d_SceneAsset Icon").image as Texture2D;

      clicked += OpenLevelStaticDataForCurrentScene;
    }

    /// <summary>
    /// Opens corresponding LevelStaticData in Inspector
    /// </summary>
    private void OpenLevelStaticDataForCurrentScene()
    {
      string currentSceneName = GetCurrentSceneName();

      var allLevelDatas = Resources.LoadAll<LevelStaticData>("StaticData/Levels");

      var matchingData = allLevelDatas.FirstOrDefault(data => data.LevelKey.Equals(currentSceneName));

      if (matchingData != null)
      {
        Selection.activeObject = matchingData;
      }
      else
      {
        Debug.LogWarning($"No LevelStaticData found for scene '{currentSceneName}'");
      }
    }

    private static string GetCurrentSceneName() =>
      Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
  }
}
#endif
