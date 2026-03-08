// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData;

using Code.Gameplay.Audio.Music.Interfaces;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenjex.Extensions.Attribute;
using Zenjex.Extensions.Injector;

namespace Code.Gameplay.Audio.Music
{
  /// <summary>
  /// Triggers a music crossfade when a tagged collider enters this zone.
  /// Place on a trigger volume in the level. Assign the target playlist in the Inspector.
  ///
  /// Example uses:
  ///   - Entering a dungeon area switches from ambient to combat music.
  ///   - Walking into a boss room fades to the boss theme.
  ///   - Opening a menu can be handled via <see cref="IMusicPlayer.CrossfadeTo"/> directly
  ///     from the window logic instead of using this component.
  /// </summary>
  [RequireComponent(typeof(Collider))]
  public class MusicZoneTrigger : ZenjexBehaviour
  {
    [Tooltip("The playlist that starts playing when the triggering collider enters this zone.")]
    public MusicPlaylist targetPlaylist;

    [Tooltip("Only colliders with this tag will activate the crossfade. " +
             "Leave empty to react to any collider.")]
    public string triggerTag = "Player";

    [Tooltip("When enabled, this trigger fires only once and then disables itself. " +
             "Useful for one-shot events like entering a boss room.")]
    public bool fireOnce = true;

    [Zenjex] private readonly IMusicPlayer _musicPlayer;

    private void OnTriggerEnter(Collider other)
    {
      if (!IsTriggeredBy(other))
        return;

      if (targetPlaylist == null)
      {
        Debug.LogWarning($"[MusicZoneTrigger] '{name}' has no target playlist assigned.", this);
        return;
      }

      _musicPlayer.CrossfadeTo(targetPlaylist).Forget();

      if (fireOnce)
        gameObject.SetActive(false);
    }

    private bool IsTriggeredBy(Collider other) =>
      string.IsNullOrEmpty(triggerTag) || other.CompareTag(triggerTag);
  }
}
