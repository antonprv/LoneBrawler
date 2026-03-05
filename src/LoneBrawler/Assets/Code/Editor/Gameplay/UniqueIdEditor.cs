// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System;
using System.Linq;

using Code.Common.UtilityComponents;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

namespace Code.Editor.Gameplay
{
  [CustomEditor(typeof(UniqueId))]
  public class UniqueIdEditor : UnityEditor.Editor
  {
    private void OnEnable()
    {
      var uniqueId = (UniqueId)target;

      if (IsPrefab(uniqueId))
        return;

      if (string.IsNullOrEmpty(uniqueId.id))
      {
        Generate(uniqueId);
      }
      else
      {
        UniqueId[] uniqueIds =
          FindObjectsByType<UniqueId>(FindObjectsSortMode.None);

        if (uniqueIds.Any(other =>
          other != uniqueId && other.id == uniqueId.id))
        {
          Generate(uniqueId);
        }
      }
    }

    private bool IsPrefab(UniqueId uniqueId)
    {
      return uniqueId.gameObject.scene.rootCount == 0;
    }

    private void Generate(UniqueId uniqueId)
    {
      uniqueId.id =
        $"{uniqueId.gameObject.scene.name}_{Guid.NewGuid().ToString()}";

      if (!Application.isPlaying)
      {
        EditorUtility.SetDirty(uniqueId);
        EditorSceneManager.MarkSceneDirty(uniqueId.gameObject.scene);
      }
    }
  }
}
