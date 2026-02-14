// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using Code.Common.Extensions.CustomTypes.Types.Editor;
using Code.Editor.Common;

using UnityEditor;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Data.StaticData.Editor
{
  /// <summary>
  /// Custom editor for EnemyManifestStaticData that provides automatic population
  /// of the Enemies dictionary from all available EnemyStaticData assets.
  /// </summary>
  [CustomEditor(typeof(EnemyManifestStaticData))]
  public class EnemyManifestStaticDataEditor : ManualSaveEditor
  {
    private DictionaryDataDrawerHelper _dictionaryHelper;
    private SerializedProperty _enemiesProperty;

    private void OnEnable()
    {
      _dictionaryHelper = new DictionaryDataDrawerHelper();
      _enemiesProperty = serializedObject.FindProperty("Enemies");
    }

    protected override void OnDisable()
    {
      _dictionaryHelper?.ClearCache();
      base.OnDisable();
    }

    protected override void DrawInspector()
    {
      serializedObject.Update();

      EditorGUILayout.Space(10);
      DrawAutoFillButton();
      EditorGUILayout.Space(10);
      DrawEnemiesDictionary();

      serializedObject.ApplyModifiedProperties();
    }

    #region UI Drawing

    /// <summary>
    /// Draws the AutoFill button that populates the dictionary with all EnemyStaticData assets.
    /// </summary>
    private void DrawAutoFillButton()
    {
      EditorGUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();

      if (GUILayout.Button("AutoFill from Assets", GUILayout.Height(30), GUILayout.Width(200)))
      {
        AutoFillEnemies();
      }

      GUILayout.FlexibleSpace();
      EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the Enemies dictionary using the reusable helper.
    /// </summary>
    private void DrawEnemiesDictionary()
    {
      if (_enemiesProperty != null)
      {
        _dictionaryHelper.DrawDictionaryLayout(_enemiesProperty, new GUIContent("Enemies"));
      }
    }

    #endregion

    #region AutoFill Logic

    /// <summary>
    /// Automatically fills the Enemies dictionary with all EnemyStaticData assets found in the project.
    /// Maps each asset by its EnemyTypeId and creates an AssetReference for it.
    /// </summary>
    private void AutoFillEnemies()
    {
      // Find all EnemyStaticData assets in the project
      string[] guids = AssetDatabase.FindAssets("t:EnemyStaticData");

      if (guids.Length == 0)
      {
        EditorUtility.DisplayDialog(
            "No Assets Found",
            "No EnemyStaticData assets were found in the project.",
            "OK"
        );
        return;
      }

      int addedCount = 0;
      int skippedCount = 0;
      int updatedCount = 0;

      // Get the target manifest
      var manifest = (EnemyManifestStaticData)target;

      // Process each found asset
      foreach (string guid in guids)
      {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        var enemyData = AssetDatabase.LoadAssetAtPath<EnemyStaticData>(assetPath);

        if (enemyData == null)
          continue;

        // Check if this EnemyTypeId already exists in the dictionary
        if (manifest.Enemies.ContainsKey(enemyData.EnemyTypeId))
        {
          // Update existing entry
          var existingReference = manifest.Enemies[enemyData.EnemyTypeId];
          string existingPath = AssetDatabase.GUIDToAssetPath(existingReference.AssetGUID);

          if (existingPath != assetPath)
          {
            manifest.Enemies[enemyData.EnemyTypeId] = CreateAssetReference(assetPath, guid);
            updatedCount++;
          }
          else
          {
            skippedCount++;
          }
        }
        else
        {
          // Add new entry
          manifest.Enemies.Add(
              enemyData.EnemyTypeId,
              CreateAssetReference(assetPath, guid)
          );
          addedCount++;
        }
      }

      // Mark the object as dirty so Unity saves the changes
      EditorUtility.SetDirty(manifest);
      serializedObject.Update();

      // Show summary dialog
      ShowAutoFillSummary(addedCount, updatedCount, skippedCount);
    }

    /// <summary>
    /// Creates an AssetReferenceT for the given asset path and GUID.
    /// </summary>
    private AssetReferenceT<EnemyStaticData> CreateAssetReference(string assetPath, string guid)
    {
      var reference = new AssetReferenceT<EnemyStaticData>(guid);
      return reference;
    }

    /// <summary>
    /// Displays a summary dialog showing the results of the AutoFill operation.
    /// </summary>
    private void ShowAutoFillSummary(int added, int updated, int skipped)
    {
      string message = $"AutoFill completed:\n\n" +
                     $"• Added: {added} new entries\n" +
                     $"• Updated: {updated} entries\n" +
                     $"• Skipped: {skipped} unchanged entries\n\n" +
                     $"Total entries in dictionary: {((EnemyManifestStaticData)target).Enemies.Count}";

      EditorUtility.DisplayDialog("AutoFill Complete", message, "OK");
    }

    #endregion
  }
}
#endif
