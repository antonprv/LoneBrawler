// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Code.Data.StaticData;
using Code.Data.StaticData.Manifests;
using Code.Infrastructure.AssetManagement.Addresses;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;

using UnityEngine;
using UnityEngine.AddressableAssets;

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
    private async void OpenLevelStaticDataForCurrentScene()
    {
      string currentSceneName = GetCurrentSceneName();

      var manifest = await Addressables.LoadAssetAsync<LevelsManifestStaticData>(StaticDataAddresses.LevelsManifestAddress).Task;

      KeyValuePair<string, AssetReferenceT<LevelStaticData>> dataAddress =
        manifest.Levels.FirstOrDefault(data => data.Key.Equals(currentSceneName));

      var levelData =
        await Addressables.LoadAssetAsync<LevelStaticData>(dataAddress.Value).Task;

      if (levelData != null)
      {
        Selection.activeObject = levelData;
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
