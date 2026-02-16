// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using Code.Common.CustomTypes.Domain.Collections.Interfaces;

using Code.Common.Extensions.CustomTypes.Types.Editor;
using Code.Editor.Common.Manifests.Interfaces;

using UnityEditor;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Editor.Common.Manifests
{
  /// <summary>
  /// Base class for manifest editors that provides common functionality for AutoFill operations.
  /// Handles dictionary drawing and automatic population from ScriptableObject assets.
  /// </summary>
  /// <typeparam name="TManifest">Type of the manifest ScriptableObject</typeparam>
  /// <typeparam name="TData">Type of the data ScriptableObject to search for</typeparam>
  /// <typeparam name="TKey">Type of the dictionary key</typeparam>
  public abstract class ManifestEditorBase<TManifest, TData, TKey> : ManualSaveEditor
      where TManifest : ScriptableObject
      where TData : ScriptableObject
  {
    private DictionaryDataDrawerHelper _dictionaryHelper;
    private ICustomKeyDrawer _customKeyDrawer;
    private SerializedProperty _dictionaryProperty;
    private bool _useCustomDrawer = false;

    #region Lifecycle

    private void OnEnable()
    {
      _dictionaryHelper = new DictionaryDataDrawerHelper();
      _dictionaryProperty = serializedObject.FindProperty(GetDictionaryPropertyName());

      // Check if custom key drawer should be used
      _customKeyDrawer = CreateCustomKeyDrawer();
      _useCustomDrawer = _customKeyDrawer != null;

      OnEnableCustom();
    }

    protected override void OnDisable()
    {
      _dictionaryHelper?.ClearCache();
      _customKeyDrawer?.ClearCache();
      OnDisableCustom();
      base.OnDisable();
    }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Gets the name of the dictionary property in the manifest.
    /// Example: "Enemies", "Levels"
    /// </summary>
    protected abstract string GetDictionaryPropertyName();

    /// <summary>
    /// Gets the display label for the dictionary in the Inspector.
    /// Example: "Enemies", "Levels"
    /// </summary>
    protected abstract string GetDictionaryDisplayLabel();

    /// <summary>
    /// Extracts the key from a data asset.
    /// Example: enemyData.EnemyTypeId, levelData.LevelKey
    /// </summary>
    protected abstract TKey GetKeyFromData(TData data);

    /// <summary>
    /// Gets the dictionary from the manifest for modification.
    /// </summary>
    protected abstract System.Collections.Generic.IDictionary<TKey, AssetReferenceT<TData>> GetDictionary(TManifest manifest);

    #endregion

    #region Virtual Methods

    /// <summary>
    /// Called during OnEnable. Override for custom initialization.
    /// </summary>
    protected virtual void OnEnableCustom() { }

    /// <summary>
    /// Called during OnDisable. Override for custom cleanup.
    /// </summary>
    protected virtual void OnDisableCustom() { }

    /// <summary>
    /// Draws additional UI before the AutoFill button. Override to add custom controls.
    /// </summary>
    protected virtual void DrawBeforeAutoFill() { }

    /// <summary>
    /// Draws additional UI after the dictionary. Override to add custom controls.
    /// </summary>
    protected virtual void DrawAfterDictionary() { }

    /// <summary>
    /// Creates a custom key drawer for the dictionary entries.
    /// Return null to use default text field for keys.
    /// Override to provide custom key input (e.g., dropdown, object picker).
    /// </summary>
    protected virtual ICustomKeyDrawer CreateCustomKeyDrawer() => null;

    #endregion

    #region Inspector Drawing

    protected override void DrawInspector()
    {
      // Note: serializedObject.Update() and ApplyModifiedProperties() 
      // are handled by ManualSaveEditor base class

      EditorGUILayout.Space(10);
      DrawBeforeAutoFill();
      DrawAutoFillButton();
      EditorGUILayout.Space(10);
      DrawDictionary();
      DrawAfterDictionary();
    }

    /// <summary>
    /// Draws the AutoFill button.
    /// </summary>
    private void DrawAutoFillButton()
    {
      EditorGUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();

      if (GUILayout.Button("AutoFill from Assets", GUILayout.Height(30), GUILayout.Width(200)))
      {
        PerformAutoFill();
      }

      GUILayout.FlexibleSpace();
      EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the dictionary using the reusable helper or custom drawer.
    /// </summary>
    private void DrawDictionary()
    {
      if (_dictionaryProperty == null)
        return;

      if (_useCustomDrawer && _customKeyDrawer != null)
      {
        // Use custom drawer with special key rendering
        _customKeyDrawer.DrawDictionaryWithCustomKeys(
            _dictionaryProperty,
            new GUIContent(GetDictionaryDisplayLabel())
        );
      }
      else
      {
        // Use default dictionary drawer
        _dictionaryHelper.DrawDictionaryLayout(
            _dictionaryProperty,
            new GUIContent(GetDictionaryDisplayLabel())
        );
      }
    }

    #endregion

    #region AutoFill Logic

    /// <summary>
    /// Performs the AutoFill operation by finding all data assets and populating the dictionary.
    /// </summary>
    private void PerformAutoFill()
    {
      string[] guids = AssetDatabase.FindAssets($"t:{typeof(TData).Name}");

      if (guids.Length == 0)
      {
        ShowNoAssetsFoundDialog();
        return;
      }

      // Record the state before making changes for Undo support
      Undo.RecordObject(target, "AutoFill Manifest");

      var result = ProcessAssets(guids);

      // Force the dictionary to synchronize with its serialized lists
      var manifest = (TManifest)target;
      var dictionary = GetDictionary(manifest);

      // Call ForceSerialization if the dictionary type supports it
      if (dictionary is IForceSerialization forceSerialization)
      {
        forceSerialization.ForceSerialization();
      }

      // Mark the target as dirty
      EditorUtility.SetDirty(target);

      // Apply modifications to serialized properties
      serializedObject.ApplyModifiedProperties();

      // Force the serialized object to update from the modified target
      serializedObject.Update();

      // Force a repaint to show changes immediately
      Repaint();

      ShowAutoFillSummary(result);
    }

    /// <summary>
    /// Processes all found assets and updates the dictionary.
    /// </summary>
    private AutoFillResult ProcessAssets(string[] guids)
    {
      var result = new AutoFillResult();
      var manifest = (TManifest)target;
      var dictionary = GetDictionary(manifest);

      foreach (string guid in guids)
      {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        var data = AssetDatabase.LoadAssetAtPath<TData>(assetPath);

        if (data == null)
          continue;

        TKey key = GetKeyFromData(data);

        if (dictionary.ContainsKey(key))
        {
          UpdateExistingEntry(dictionary, key, assetPath, guid, ref result);
        }
        else
        {
          AddNewEntry(dictionary, key, assetPath, guid, ref result);
        }
      }

      return result;
    }

    /// <summary>
    /// Updates an existing dictionary entry if the asset path has changed.
    /// </summary>
    private void UpdateExistingEntry(
        System.Collections.Generic.IDictionary<TKey, AssetReferenceT<TData>> dictionary,
        TKey key,
        string assetPath,
        string guid,
        ref AutoFillResult result)
    {
      var existingReference = dictionary[key];
      string existingPath = AssetDatabase.GUIDToAssetPath(existingReference.AssetGUID);

      if (existingPath != assetPath)
      {
        dictionary[key] = new AssetReferenceT<TData>(guid);
        result.UpdatedCount++;
      }
      else
      {
        result.SkippedCount++;
      }
    }

    /// <summary>
    /// Adds a new entry to the dictionary.
    /// </summary>
    private void AddNewEntry(
        System.Collections.Generic.IDictionary<TKey, AssetReferenceT<TData>> dictionary,
        TKey key,
        string assetPath,
        string guid,
        ref AutoFillResult result)
    {
      dictionary.Add(key, new AssetReferenceT<TData>(guid));
      result.AddedCount++;
    }

    #endregion

    #region Dialogs

    /// <summary>
    /// Shows a dialog when no assets are found.
    /// </summary>
    private void ShowNoAssetsFoundDialog()
    {
      EditorUtility.DisplayDialog(
          "No Assets Found",
          $"No {typeof(TData).Name} assets were found in the project.",
          "OK"
      );
    }

    /// <summary>
    /// Shows a summary dialog with the results of the AutoFill operation.
    /// </summary>
    private void ShowAutoFillSummary(AutoFillResult result)
    {
      var manifest = (TManifest)target;
      var dictionary = GetDictionary(manifest);

      string message = $"AutoFill completed:\n\n" +
                     $"• Added: {result.AddedCount} new entries\n" +
                     $"• Updated: {result.UpdatedCount} entries\n" +
                     $"• Skipped: {result.SkippedCount} unchanged entries\n\n" +
                     $"Total entries in dictionary: {dictionary.Count}";

      EditorUtility.DisplayDialog("AutoFill Complete", message, "OK");
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// Stores the results of an AutoFill operation.
    /// </summary>
    private struct AutoFillResult
    {
      public int AddedCount;
      public int UpdatedCount;
      public int SkippedCount;
    }

    #endregion
  }

  #region Custom Key Drawer Support

  #endregion
}
#endif
