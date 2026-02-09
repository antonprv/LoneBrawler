// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEditor;

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
  }
}
