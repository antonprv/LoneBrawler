// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

#if UNITY_EDITOR
using Code.Editor.Tools.LevelInspector;

using UnityEditor;
using UnityEditor.Overlays;

namespace Code.Editor.Tools.SceneDataSelector
{
  // Добавляем оверлей для Scene View
  [Overlay(typeof(SceneView), "Level Inspector", true)]
  public sealed class SceneDataSelectorOverlay : ToolbarOverlay
  {
    public SceneDataSelectorOverlay() : base(SceneDataSelectorButton.ID) { }
  }
}
#endif
