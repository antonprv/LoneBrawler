// Created by Anton Piruev in 2025. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;

using UnityEngine;
namespace Code.Editor.Tools.SceneSwitcher
{
  [EditorToolbarElement(ID, typeof(SceneView))]
  public sealed class SceneSwitcherDropdown : EditorToolbarDropdown
  {
    public const string ID = "SceneSwitcher/Dropdown";

    public SceneSwitcherDropdown()
    {
      text = "Scene";
      tooltip = "Switch active scene";

      icon = EditorGUIUtility.IconContent("SceneAsset Icon").image as Texture2D;

      clicked += ShowSceneMenu;
    }

    private void ShowSceneMenu()
    {
      if (EditorApplication.isPlayingOrWillChangePlaymode)
        return;

      var menu = new GenericMenu();

      string currentScene = GetCurrentSceneName();
      string[] scenes = GetAllScenes();

      foreach (string scene in scenes)
      {
        bool isActive = scene == currentScene;

        menu.AddItem(
          new GUIContent(scene),
          isActive,
          () => LoadScene(scene)
        );
      }

      menu.ShowAsContext();
    }

    private static void LoadScene(string sceneName)
    {
      string path = FindScenePath(sceneName);
      if (string.IsNullOrEmpty(path))
        return;

      if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
      {
        EditorSceneManager.OpenScene(path);
      }
    }

    private static string[] GetAllScenes()
    {
      return Directory
        .GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
        .Select(Path.GetFileNameWithoutExtension)
        .OrderBy(n => n)
        .ToArray();
    }

    private static string GetCurrentSceneName()
    {
      return Path.GetFileNameWithoutExtension(
        EditorSceneManager.GetActiveScene().path
      );
    }

    private static string FindScenePath(string sceneName)
    {
      return Directory
        .GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
        .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == sceneName);
    }
  }
}

#endif
